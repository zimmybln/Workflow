using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Designer.Contracts
{
    public interface IDesignerOptionsExtension
    {
        T GetOptions<T>()
            where T : class ;
    }
}
