using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Designer.Models
{
    public interface IWriterAdapter
    {
        void WriteLine(string message);
    }
}
