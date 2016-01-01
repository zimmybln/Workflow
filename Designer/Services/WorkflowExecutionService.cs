using System;
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
            
           // Trace.Listeners.AddRange(options.TraceListeners.ToArray());

            app.Run();

            
        }
    }   
}
