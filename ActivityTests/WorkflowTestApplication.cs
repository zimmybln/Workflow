using System;
using System.Activities;
using System.Activities.DurableInstancing;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace ActivityTests
{
    public class WorkflowTestApplication
    {
        private readonly Activity _activityToTest;

        public WorkflowTestApplication(Activity activity)
        {
            _activityToTest = activity;
        }

        public Dictionary<string, object> Inputs { get; } = new Dictionary<string, object>();

        public WorkflowTestResult Run(params object[] extensions)
        {
            WorkflowApplication wa;
            
            wa = new WorkflowApplication(_activityToTest, Inputs);

            foreach (var extension in extensions)
            {
                wa.Extensions.Add(extension);
            }
            
            var result = new WorkflowTestResult();

            var autoreset = new AutoResetEvent(false);

            // Setzen der Ereignisse

            wa.Aborted += delegate(WorkflowApplicationAbortedEventArgs args)
            {
                autoreset.Set();
            };

            wa.Unloaded += delegate(WorkflowApplicationEventArgs args)
            {
                autoreset.Set();
            };

            wa.Completed += delegate(WorkflowApplicationCompletedEventArgs args)
            {
                if (args.CompletionState == ActivityInstanceState.Closed)
                {
                    foreach (var item in args.Outputs)
                    {
                       result.Outputs.Add(item.Key, item.Value);
                    }
                }

                autoreset.Set();
            };

            wa.OnUnhandledException += delegate(WorkflowApplicationUnhandledExceptionEventArgs args)
            {
                result.Exception = args.UnhandledException;

                autoreset.Set();

                return UnhandledExceptionAction.Terminate;

            };

            wa.PersistableIdle += delegate(WorkflowApplicationIdleEventArgs args)
            {

                autoreset.Set();

                return PersistableIdleAction.Unload;
            };

            wa.Run();

            autoreset.WaitOne();
            

            result.Id = wa.Id;
 
            return result;

 

        }
        }



}

