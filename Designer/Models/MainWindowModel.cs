using Designer.Components;
using System;
using System.Activities;
using System.Activities.Statements;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Designer.Components.Workflow;
using Designer.Services;

namespace Designer.Models
{
    public class MainWindowModel : ModelBase, IWriterAdapter
    {
        private Activity _currentworkflow = null;

        public MainWindowModel()
        {
            ExecuteWorkflow = new MethodCommand(OnExecuteWorkflow);
            CloseCommand = new MethodCommand(OnCloseCommand);
           
            ToolboxItems.AddRange("Default", typeof(If), typeof(Sequence), typeof(While), typeof(DoWhile), typeof(Assign), typeof(Switch<>), typeof(WriteLine),
                                            typeof(TerminateWorkflow), typeof(Delay), typeof(InvokeMethod));
            ToolboxItems.AddRange("ErrorHandling", typeof(Throw), typeof(TryCatch), typeof(Rethrow));

            var notifications = ApplicationServices.GetService<NotificationService>();

            if (notifications != null)
            {
                notifications.OnNotify += OnNotify;
            }
            
        }

        private void OnExecuteWorkflow(ICommand command, object o)
        {
            // reset all the UI Output
            TraceMessages.Clear();

            // to catch output messages we have to redirect the console outputstream
            Console.SetOut(new TextToUiWriter(this));
            
            // execute the workflow
            var workflowexecution = ApplicationServices.GetService<IWorkflowExecutionService>();

            var options = new WorkflowExecutionOptions();

            var executable = CurrentWorkflow.Clone();

            workflowexecution.Execute(executable, options);
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

        public ToolboxItemDescriptorCollection ToolboxItems { get; } = new ToolboxItemDescriptorCollection();
        
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
