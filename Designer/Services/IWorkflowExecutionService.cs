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
using Designer.Contracts;
using Designer.Models;

namespace Designer.Services
{
    public class WorkflowExecutionOptions
    {
        public IWriterAdapter TraceWriter { get; set; } = null;
        
        public List<TrackingParticipant> TrackingParticipants { get; } = new List<TrackingParticipant>();
        
        public IDesignerOptionsExtension DesignerOptions { get; set; } 
    }


    public interface IWorkflowExecutionService : IService
    {
        Task Execute(Activity activity, WorkflowExecutionOptions options);
    }
}
