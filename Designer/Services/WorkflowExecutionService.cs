using System;
using System.Activities;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Designer.Models;
using Prism.Modularity;

namespace Designer.Services
{
    [Export(typeof(IWorkflowExecutionService))]
    public class WorkflowExecutionService : IWorkflowExecutionService, IModule
    {
        public Task Execute(Activity activity, WorkflowExecutionOptions options)
        {
            return Task.Run(() => 
            {
                AutoResetEvent resetevent = new AutoResetEvent(false);
                IWriterAdapter writer = null;

                var app = new WorkflowApplication(activity);

                if (options != null)
                {
                    writer = options.TraceWriter;

                    // add all tracking participants
                    options.TrackingParticipants.ForEach(tp => app.Extensions.Add(tp));
                }

                app.Completed = (args =>
                {
                    writer?.WriteLine($"Workflowapplication action: completed as {args.CompletionState}");
                });


                app.Aborted = (args =>
                {
                    writer?.WriteLine($"Workflowapplication action: aborted {args.Reason.Message}");
                    resetevent.Set();
                });

                app.OnUnhandledException = (args =>
                {
                    writer?.WriteLine("Workflowapplication action: unhandled exception");
                    return UnhandledExceptionAction.Cancel;
                });

                app.Unloaded = (args =>
                {
                    writer?.WriteLine("Workflowapplication action: unloaded");
                    resetevent.Set();
                });

                app.OnUnhandledException = (args =>
                {
                    writer?.WriteLine($"Workflowapplication action: unhandled exception {args.UnhandledException.Message}");
                    return UnhandledExceptionAction.Cancel;
                });

                app.PersistableIdle = (args =>
                {
                    return PersistableIdleAction.Unload;
                });

                app.Run();

                // wait for finish the execution
                resetevent.WaitOne();
            });
        }

        public void Initialize()
        {
            
        }
    }   
}
