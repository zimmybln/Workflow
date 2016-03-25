using System;
using System.Activities.Tracking;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Designer.Models;

namespace Designer.Components.Workflow
{
    [Flags]
    public enum StateChangeRecords
    {
        None = 0x0000,

        ActivityScheduled = 0x0001,

        ActivityState = 0x0002,

        BookmarkResumption = 0x0004,

        CancelRequested = 0x0008,

        FaultPropagation = 0x0010,

        WorkflowInstance = 0x0020,

        All = ActivityScheduled
            | ActivityState 
            | BookmarkResumption 
            | CancelRequested 
            | FaultPropagation 
            | WorkflowInstance
    }

    
    public class StateChangeTrackingParticipant : TrackingParticipant
    {
        private readonly IWriterAdapter _writerAdapter;
        private readonly StateChangeRecords _stateChangeRecords;

        private DateTime _startTime;

        public StateChangeTrackingParticipant(IWriterAdapter writer, StateChangeRecords records)
        {
            _writerAdapter = writer;
            _stateChangeRecords = records;
        }

        protected override void Track(TrackingRecord record, TimeSpan timeout)
        {
            TimeSpan timeSpan;
            LoggingEntry entry = null;

            if (record.RecordNumber == 0)
            {
                _startTime = record.EventTime.ToLocalTime();
                timeSpan = new TimeSpan(0,0,0,0,0);
            }
            else
            {
                timeSpan = record.EventTime.ToLocalTime() - _startTime;
            }

            var timeSpanValue = timeSpan.ToString("mm\\:ss\\.fff");
            var commoninfo = $"{record.RecordNumber:00000} {timeSpanValue}";


            if (IsSet(StateChangeRecords.ActivityState) && record is ActivityStateRecord)
            {
                var staterecord = (ActivityStateRecord) record;

                entry = new LoggingEntry($"{commoninfo} {record.GetType().Name} {staterecord.Activity.Name} ({staterecord.Activity.TypeName}) {staterecord.State}");

                // add each variable/value to 
                foreach (var valuePair in staterecord.Variables)
                {
                    entry.AdditionalInformations.Add(new PropertyEntry() {Name = valuePair.Key, Value = valuePair.Value});
                }
            } 
            else if (IsSet(StateChangeRecords.ActivityScheduled) && record is ActivityScheduledRecord)
            {
                var scheduledrecord = (ActivityScheduledRecord) record;

                _writerAdapter.WriteLine($"{commoninfo} {scheduledrecord?.Activity?.Name} ({scheduledrecord?.Activity?.TypeName}) Child: {scheduledrecord.Child.Name}");
            } 
            else if (IsSet(StateChangeRecords.WorkflowInstance) && record is WorkflowInstanceRecord)
            {
                var instancerecord = (WorkflowInstanceRecord) record;

                _writerAdapter.WriteLine($"{commoninfo} State changed: {instancerecord.State}");
            }
            else if (IsSet(StateChangeRecords.CancelRequested) && record is CancelRequestedRecord)
            {
                var cancelrecord = (CancelRequestedRecord) record;

                _writerAdapter.WriteLine($"{commoninfo} {record.GetType().Name} {cancelrecord.Activity?.Name}");
            } 
            else if (IsSet(StateChangeRecords.FaultPropagation) && record is FaultPropagationRecord)
            {
                var faultrecord = (FaultPropagationRecord) record;

                _writerAdapter.WriteLine($"{commoninfo} Fault {faultrecord?.Fault.Message}");
            }

            if (record.Annotations.Any())
            {
                Debug.WriteLine($"Anmerkungen bei {record.RecordNumber}");
            }

            if (entry != null)
            {


                entry.Annotations.AddRange(record.Annotations.Values.ToList());
               _writerAdapter.WriteEntry(entry); 
            }
            
               
        }

        protected bool IsSet(StateChangeRecords record)
        {
            return (_stateChangeRecords & record) == record;
        }
    }
}
