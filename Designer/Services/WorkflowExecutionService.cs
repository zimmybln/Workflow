using System;
using System.Activities;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;

namespace Designer.Services
{
    [Export(typeof(IWorkflowExecutionService))]
    public class WorkflowExecutionService : IWorkflowExecutionService
    {
        public void Execute(Activity activity)
        {
            var options = new WorkflowExecutionOptions()
            {
                // some default settings
            }; 

            Execute(activity, options);
        }

        public void Execute(Activity activity, WorkflowExecutionOptions options)
        {
            
            var app = new WorkflowApplication(activity);
            
            if (options != null)
            {
                // add all tracking participants
                options.TrackingParticipants.ForEach(tp => app.Extensions.Add(tp));
            }

            app.Run();

            
        }
    }   
}
