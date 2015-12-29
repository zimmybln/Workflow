using System.Activities;
using System.ComponentModel.Composition;
using System.Diagnostics;

namespace Designer.Services
{
    [Export(typeof(IWorkflowExecutionService))]
    public class WorkflowExecutionService : IWorkflowExecutionService
    {
        public void Execute(Activity activity)
        {
            var options = new WorkflowExecutionOptions()
            {

            }; 

            Execute(activity, options);
        }

        public void Execute(Activity activity, WorkflowExecutionOptions options)
        {
            var app = new WorkflowApplication(activity);

            Trace.Listeners.Add(new DebugListener());

            app.Run();

        }
    }

    public class DebugListener : TraceListener
    {
        public override void Write(string message)
        {
            Debug.Write(message);
        }

        public override void WriteLine(string message)
        {
            Debug.WriteLine(message);
        }
    }


    
}
