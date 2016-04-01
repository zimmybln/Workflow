using System;
using System.Activities.Tracking;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Designer.Components.Workflow
{
    public class RelativeExecutionTimeProvider : ExecutionTimeProvider
    {
        private DateTime _startTime;


        public override string GetTimeInfo(TrackingRecord record)
        {
            TimeSpan timeSpan;

            if (record.RecordNumber == 0)
            {
                _startTime = record.EventTime.ToLocalTime();
                timeSpan = new TimeSpan(0, 0, 0, 0, 0);
            }
            else
            {
                timeSpan = record.EventTime.ToLocalTime() - _startTime;
            }

            return timeSpan.ToString("mm\\:ss\\.fff");
        }
    }
}
