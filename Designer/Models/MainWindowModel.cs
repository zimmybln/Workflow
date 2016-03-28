using Designer.Components;
using System;
using System.Activities;
using System.Activities.Statements;
using System.Activities.XamlIntegration;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xaml;
using Designer.Components.Workflow;
using Designer.Dialogs;
using Designer.Properties;
using Designer.Services;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Win32;
using Prism.Modularity;
using Prism.Mvvm;
using XamlWriter = System.Windows.Markup.XamlWriter;

namespace Designer.Models
{

    [Export]
    public class MainWindowModel : BindableBase, IWriterAdapter, IPartImportsSatisfiedNotification
    {
        private Activity _currentworkflow = null;
        private bool _isBackstageOpen;
        private Dispatcher _dispatcher;
        private LoadedAdapter _loadedadapter;
        private Timer _timer;

        [Import(AllowRecomposition = false)]
        private IModuleCatalog moduleCatalog;

        [Import(AllowRecomposition = false)]
        private IModuleManager moduleManager;

        [Import(AllowRecomposition = false)]
        private NotificationService notificationService;

        [Import(AllowRecomposition = true)]
        private IWorkflowExecutionService executionService;

        public MainWindowModel() 
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            
            ExecuteWorkflow = new MethodCommand(OnExecuteWorkflow);
            CloseCommand = new MethodCommand(OnCloseCommand);
            SaveWorkflowCommand = new MethodCommand(OnSaveWorkflowCommand);
            LoadWorkflowCommand = new MethodCommand(OnLoadWorkflowCommand);
            ShowTraceDetailsCommand = new MethodCommand(OnShowTraceDetailsCommand);

            _loadedadapter = new LoadedAdapter(OnLoaded);

            ToolboxItems.AddRange("Default", typeof(If), typeof(Sequence), typeof(While), typeof(DoWhile), typeof(Assign), typeof(Switch<>), typeof(WriteLine),
                                            typeof(TerminateWorkflow), typeof(Delay), typeof(InvokeMethod));
            ToolboxItems.AddRange("ErrorHandling", typeof(Throw), typeof(TryCatch), typeof(Rethrow));

            if (Settings.Default.LoadActivities)
            {
                ToolboxItems.AddFromDirectory("Activities");
            }
                      
            Settings.Default.PropertyChanged += OnSettingsPropertyChanged;
        }

        private void OnSettingsPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName.Equals(nameof(Settings.Default.LoadActivities)))
            {
                
            }
            
        }

        private async void OnExecuteWorkflow(ICommand command, object o)
        { 
            // reset all the UI Output
            TraceMessages.Clear();

            // to catch output messages we have to redirect the console outputstream
            Console.SetOut(new TextToUiWriter(this) { Prefix = "Console: " });
            
            // execute the workflow
            var options = new WorkflowExecutionOptions()
            {
                TraceWriter = this
            };

            options.TrackingParticipants.Add(new StateChangeTrackingParticipant(this, StateChangeRecords.All));
            
            var executable = CurrentWorkflow.Clone();

            await executionService.Execute(executable, options);

            notificationService.Notify(new ExecutionFinishedNotification("Execution finished"));
            
        }

        private void OnShowTraceDetailsCommand(ICommand command, object parameter)
        {
            var entry = parameter as LoggingEntry;

            if (entry == null) return;

            var dlg = new LoggingEntryProperties(entry);

            dlg.ShowDialog();
        }

        private void OnSaveWorkflowCommand(ICommand command, object parameter)
        {
            // select a filename
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Workflow Definition File (*.wdef)|*.wdef|All files|*.*";
            dlg.FilterIndex = 0;
            dlg.OverwritePrompt = true;
            dlg.AddExtension = true;
            dlg.CheckPathExists = true;
            dlg.DefaultExt = "wdef";

            if (dlg.ShowDialog() != true)
                return;

            // save the current workflow
            if (File.Exists(dlg.FileName))
                File.Delete(dlg.FileName);

            StreamWriter sw = File.CreateText(dlg.FileName);
            var writer = ActivityXamlServices.CreateBuilderWriter(new XamlXmlWriter(sw, new XamlSchemaContext()));
            XamlServices.Save(writer, CurrentWorkflow);
            sw.Close();

            IsBackstageOpen = false;


        }

        private void OnLoadWorkflowCommand(ICommand command, object parameter)
        {
            // select a filename
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Workflow Definition File (*.wdef)|*.wdef|All files|*.*";
            dlg.FilterIndex = 0;
            dlg.AddExtension = true;
            dlg.CheckPathExists = true;
            dlg.CheckFileExists = true;
            dlg.DefaultExt = "wdef";

            if (dlg.ShowDialog() != true)
                return;

            StreamReader sr = File.OpenText(dlg.FileName);
            var reader = ActivityXamlServices.CreateBuilderReader(new XamlXmlReader(sr, new XamlSchemaContext()));
            var workflow = XamlServices.Load(reader) as Activity;
            CurrentWorkflow = workflow;

            IsBackstageOpen = false;

        }

        private void OnCloseCommand(ICommand command, object o)
        {
            Application.Current.Shutdown();
        }

        private void OnNotify(object sender, INotification notification)
        {
            if (notification is INotificationMessage)
            {
                // reset the timer to zero
                _timer?.Change(0, 0);

                StateMessage = ((INotificationMessage) notification).Message;

                if (_timer == null)
                {
                    _timer = new Timer(OnTimerCallback);
                }
                _timer.Change(5000, 0);
            }
        }

        private void OnTimerCallback(object state)
        {
            if (!_dispatcher.CheckAccess())
            {
                _dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)(() => { notificationService.Notify(new ResetMessageNotification()); }));
            }
            else
            {
                notificationService.Notify(new ResetMessageNotification());
            }
        }


        protected void OnLoaded()
        {
            CurrentWorkflow = CreateDefaultWorkflow();

            if (notificationService != null)
            {
                Debug.WriteLine("Erfolgreich");
            }
        }

        private Activity CreateDefaultWorkflow()
        {
            return new Flowchart() { DisplayName = "Default" };
        }

        void IWriterAdapter.WriteLine(string message)
        {
            if (!_dispatcher.CheckAccess())
            {
                _dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)(() => { AddEntry(new LoggingEntry(message)); }));
            }
            else
            {
                AddEntry(new LoggingEntry(message));
            }
            
        }

        void IWriterAdapter.WriteEntry(LoggingEntry entry)
        {
            if (!_dispatcher.CheckAccess())
            {
                _dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)(() => { AddEntry(entry); }));
            }
            else
            {
                AddEntry(entry);
            }

        }

        private void AddEntry(LoggingEntry entry)
        {
            TraceMessages.Add(entry);
        }



        #region Properties

        public LoadedAdapter LoadedAdapter
        {
            get { return _loadedadapter; }
        }

        public ICommand ExecuteWorkflow { get; }

        public ICommand CloseCommand { get; }

        public ICommand SaveWorkflowCommand { get; }

        public ICommand LoadWorkflowCommand { get; }

        public ICommand ShowTraceDetailsCommand { get; }

        public ToolboxItemDescriptorCollection ToolboxItems { get; } = new ToolboxItemDescriptorCollection();

        public ObservableCollection<string> TemplateItems { get; } = new ObservableCollection<string>(); 
        
        public ObservableCollection<LoggingEntry> TraceMessages { get; } = new ObservableCollection<LoggingEntry>();

        public Activity CurrentWorkflow
        {
            get { return _currentworkflow; }
            set { SetProperty(ref _currentworkflow, value);}
        }

        public bool IsBackstageOpen
        {
            get { return _isBackstageOpen;}
            set { SetProperty(ref this._isBackstageOpen, value); }
        }

        private string _title;

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref this._title, value);}
        }

        private string _stateMessage;

        public string StateMessage
        {
            get { return _stateMessage;}
            set { SetProperty(ref this._stateMessage, value); }    
        }

        #endregion

        public void OnImportsSatisfied()
        {
            if (notificationService != null)
            {
                notificationService.OnNotify += OnNotify;
            }
        }
    }
}
