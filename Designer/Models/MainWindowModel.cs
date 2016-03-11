using Designer.Components;
using System;
using System.Activities;
using System.Activities.Statements;
using System.Activities.XamlIntegration;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Xaml;
using Designer.Components.Workflow;
using Designer.Properties;
using Designer.Services;
using Microsoft.Win32;
using XamlWriter = System.Windows.Markup.XamlWriter;

namespace Designer.Models
{
    public class MainWindowModel : ModelBase, IWriterAdapter
    {
        private Activity _currentworkflow = null;
        private bool _isBackstageOpen;

        public MainWindowModel()
        {
            ExecuteWorkflow = new MethodCommand(OnExecuteWorkflow);
            CloseCommand = new MethodCommand(OnCloseCommand);
            SaveWorkflowCommand = new MethodCommand(OnSaveWorkflowCommand);
            LoadWorkflowCommand = new MethodCommand(OnLoadWorkflowCommand);
           
            ToolboxItems.AddRange("Default", typeof(If), typeof(Sequence), typeof(While), typeof(DoWhile), typeof(Assign), typeof(Switch<>), typeof(WriteLine),
                                            typeof(TerminateWorkflow), typeof(Delay), typeof(InvokeMethod));
            ToolboxItems.AddRange("ErrorHandling", typeof(Throw), typeof(TryCatch), typeof(Rethrow));

            if (Settings.Default.LoadActivities)
            {
                ToolboxItems.AddFromDirectory("Activities");
            }
            
            
            Settings.Default.PropertyChanged += OnSettingsPropertyChanged;

            var notifications = ApplicationServices.GetService<NotificationService>();

            if (notifications != null)
            {
                notifications.OnNotify += OnNotify;
            }

            PropertyChanged += (sender, args) => Debug.WriteLine(args.PropertyName);

        }

        private void OnSettingsPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName.Equals(nameof(Settings.Default.LoadActivities)))
            {
                
            }
            
        }

        private void OnExecuteWorkflow(ICommand command, object o)
        {
            // reset all the UI Output
            TraceMessages.Clear();

            // to catch output messages we have to redirect the console outputstream
            Console.SetOut(new TextToUiWriter(this) { Prefix = "Console: " });
            
            // execute the workflow
            var workflowexecution = ApplicationServices.GetService<IWorkflowExecutionService>();

            var options = new WorkflowExecutionOptions();

            options.TrackingParticipants.Add(new StateChangeTrackingParticipant(this));
            
            var executable = CurrentWorkflow.Clone();

            workflowexecution.Execute(executable, options);
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
            
        }


        protected override void OnLoaded()
        {
            CurrentWorkflow = CreateDefaultWorkflow();
        }

        private Activity CreateDefaultWorkflow()
        {
            return new Flowchart() { DisplayName = "Default" };
        }

        void IWriterAdapter.WriteLine(string message)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => TraceMessages.Add(message));
            }
            else
            {
                TraceMessages.Add(message);
            }
            
        }

        #region Properties

        public ICommand ExecuteWorkflow { get; }

        public ICommand CloseCommand { get; }

        public ICommand SaveWorkflowCommand { get; }

        public ICommand LoadWorkflowCommand { get; }

        public ToolboxItemDescriptorCollection ToolboxItems { get; } = new ToolboxItemDescriptorCollection();

        public ObservableCollection<string> TemplateItems { get; } = new ObservableCollection<string>(); 
        
        public ObservableCollection<string> TraceMessages { get; } = new ObservableCollection<string>(); 

        public Activity CurrentWorkflow
        {
            get { return _currentworkflow; }
            set
            {
                if (!object.Equals(value, _currentworkflow))
                {
                    _currentworkflow = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool IsBackstageOpen
        {
            get { return _isBackstageOpen;}
            set
            {
                if (_isBackstageOpen != value)
                {
                    _isBackstageOpen = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _title;

        public string Title
        {
            get { return _title; }
            set
            {
                if (String.CompareOrdinal(value, _title) != 0)
                {
                    _title = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

    }
}
