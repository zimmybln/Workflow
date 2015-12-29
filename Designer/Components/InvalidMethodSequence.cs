using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Designer.Components
{
    public class InvalidMethodSequence : Exception
    {
        public InvalidMethodSequence(string ExpectedMethodName)
        {
            this.ExpectedMethod = ExpectedMethodName;
        }

        public string ExpectedMethod { get; private set; }
    }
}
