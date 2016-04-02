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
using Designer.Components.Models;
using Designer.Components.Workflow;
using Designer.Dialogs;
using Designer.Properties;
using Designer.Services;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Modularity;
using Prism.Mvvm;
using XamlWriter = System.Windows.Markup.XamlWriter;

namespace Designer.Models
{

    [Export]
    public class MainWindowModel : BindableBase, IWriterAdapter, IPartImportsSatisfiedNotification, IDocumentProvider<Activity>
    {
        private Activity _currentworkflow = null;
        private bool _isBackstageOpen;
        private bool _changed = false;
        private readonly Dispatcher _dispatcher;
        private readonly LoadedAdapter _loadedadapter;

        private readonly NotificationService _notificationService;

        [Import(AllowRecomposition = true)]
        private IWorkflowExecutionService executionService = null;

        [ImportingConstructor]
        public MainWindowModel(NotificationService notificationService)
        {
            _notificationService = notificationService;
            
            _dispatcher = Dispatcher.CurrentDispatcher;
            
            ExecuteWorkflow = new DelegateCommand(OnExecuteWorkflow);
            CloseCommand = new DelegateCommand(OnCloseCommand);
            ShowTraceDetailsCommand = new DelegateCommand<LoggingEntry>(OnShowTraceDetailsCommand);

            // Initialize model components
            StatusBar = new StatusBarModel(notificationService);

            FileModel = new ActivityFileModel(new FileBasedStorageUi() {DefaultExtension = "wdef"},
                                                   this);
            
            _loadedadapter = new LoadedAdapter(OnLoaded);

            ToolboxItems.AddRange("Default", typeof(If), typeof(Sequence), typeof(While), typeof(DoWhile), typeof(Assign), typeof(Switch<>), typeof(WriteLine),
                                            typeof(TerminateWorkflow), typeof(Delay), typeof(InvokeMethod));
            ToolboxItems.AddRange("ErrorHandling", typeof(Throw), typeof(TryCatch), typeof(Rethrow));

            if (Settings.Default.LoadActivities)
            {
                ToolboxItems.AddFromDirectory("Activities");
            }
                      
            Settings.Default.PropertyChanged += OnSettingsPropertyChanged;

            if (_notificationService != null)
            {
                _notificationService.OnNotify += OnNotify;
            }

            Title = ApplicationServices.Title;
        }

        #region IDocumentProvider

        Activity IDocumentProvider<Activity>.GetDefaultDocument()
        {
            return new Flowchart() { DisplayName = "Default" };
        }

        void IDocumentProvider<Activity>.SetDocument(Activity document)
        {
            CurrentWorkflow = document;
        }

        public Activity GetDocument()
        {
            return CurrentWorkflow;
        }

        public bool GetDocumentChanged()
        {
            return Changed;
        }

        public void ActionPerformed()
        {
            Changed = false;
            IsBackstageOpen = false;
        }

        #endregion

        private void OnSettingsPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName.Equals(nameof(Settings.Default.LoadActivities)))
            {
                 
            }
            
        }

        private async void OnExecuteWorkflow()
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

            options.TrackingParticipants.Add(new StateChangeTrackingParticipant(this, StateChangeRecords.All) {ExecutionTime = new RelativeExecutionTimeProvider()});
            
            var executable = CurrentWorkflow.Clone();

            await executionService.Execute(executable, options);

            _notificationService.Notify(new ExecutionFinishedNotification("Execution finished"));
            
        }

        private void OnShowTraceDetailsCommand(LoggingEntry parameter)
        {
            var entry = parameter as LoggingEntry;

            if (entry == null) return;

            var dlg = new LoggingEntryProperties(entry);

            dlg.ShowDialog();
        }

        private void OnCloseCommand()
        {
            Application.Current.Shutdown();
        }

        private void OnNotify(object sender, INotification notification)
        {

        }

        protected void OnLoaded()
        {
            // execute new document when loaded
            FileModel.NewCommand.Execute(null);
        }

        #region IWriterAdapter & Tracing

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

        #endregion
        
        #region Properties

        public LoadedAdapter LoadedAdapter
        {
            get { return _loadedadapter; }
        }

        public ICommand ExecuteWorkflow { get; }

        public ICommand CloseCommand { get; }

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

        public bool Changed
        {
            get { return _changed; }
            set { SetProperty(ref this._changed, value); }
        }

        private string _title;

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref this._title, value);}
        }

        public StatusBarModel StatusBar { get; }

        public StorageModel<Activity> FileModel { get; } 

        #endregion

        public void OnImportsSatisfied()
        {

        }



    }
}
