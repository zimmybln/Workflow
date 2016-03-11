using System;
using System.Activities;
using System.Activities.Tracking;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Designer.Components;

namespace Designer.Services
{
    public class WorkflowExecutionOptions
    {
        public List<TrackingParticipant> TrackingParticipants { get; } = new List<TrackingParticipant>(); 
    }


    public interface IWorkflowExecutionService : IService
    {
        void Execute(Activity activity);

        void Execute(Activity activity, WorkflowExecutionOptions options);
    }
}
