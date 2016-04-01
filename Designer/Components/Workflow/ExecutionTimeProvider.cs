using System;
using System.Activities.Tracking;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Designer.Components.Workflow
{
    public abstract class ExecutionTimeProvider
    {
        public abstract string GetTimeInfo(TrackingRecord record);
    }
}
