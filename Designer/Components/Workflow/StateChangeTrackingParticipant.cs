using System;
using System.Activities.Tracking;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Designer.Models;

namespace Designer.Components.Workflow
{
    public class StateChangeTrackingParticipant : TrackingParticipant
    {
        private readonly IWriterAdapter _writerAdapter;

        public StateChangeTrackingParticipant(IWriterAdapter writer)
        {
            _writerAdapter = writer;
        }

        protected override void Track(TrackingRecord record, TimeSpan timeout)
        {
            // here only ActivityStateRecord will be tracked
            if (record is ActivityStateRecord)
            {
                var staterecord = (ActivityStateRecord) record;

                _writerAdapter.WriteLine($"{record.RecordNumber:00000} {staterecord.Activity.Name} ({staterecord.Activity.TypeName}) {staterecord.State}");
            }           
        }


    }
}
