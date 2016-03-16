using System;
using System.Activities;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Designer.Models;

namespace Designer.Services
{
    [Export(typeof(IWorkflowExecutionService))]
    public class WorkflowExecutionService : IWorkflowExecutionService
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
                    writer?.WriteLine($"Workflow completed as {args.CompletionState}");
                });


                app.Aborted = (args =>
                {
                    resetevent.Set();
                });

                app.OnUnhandledException = (args =>
                {
                    writer?.WriteLine("Unhandled exception");
                    return UnhandledExceptionAction.Cancel;
                });

                app.Unloaded = (args =>
                {
                    writer?.WriteLine("Workflow unloaded");
                    resetevent.Set();
                });

                app.OnUnhandledException = (args =>
                {
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
    }   
}
