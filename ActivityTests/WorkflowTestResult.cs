using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivityTests
{
    public class WorkflowTestResult
    {
        public WorkflowTestResult()
        {
            Outputs = new Dictionary<string, object>();
        }

        public Guid Id { get; set; }

        public Dictionary<string, object> Outputs { get; }

        public object this[string key] => this.Outputs[key];
        
        public T GetOutput<T>(string key)
            where T : class
        {
            return Outputs[key] as T;
        }

        public Exception Exception { get; set; }
    }
}
