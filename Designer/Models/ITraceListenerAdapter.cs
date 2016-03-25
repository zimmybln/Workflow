using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Designer.Models
{
    public class PropertyEntry
    {
        public string Name { get; set; }

        public object Value { get; set; }
    }


    public class LoggingEntry
    {
        public LoggingEntry(string text)
        {
            Text = text;
            AdditionalInformations = new List<PropertyEntry>();
            Annotations = new List<string>();
        }
        
        public string Text { get; set; }

        public List<PropertyEntry> AdditionalInformations { get; }

        public List<string> Annotations { get; } 
    }



    public interface IWriterAdapter
    {
        void WriteLine(string message);

        void WriteEntry(LoggingEntry entry);
    }
}
