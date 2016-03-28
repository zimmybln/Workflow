using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Designer.Components;
using Prism.Mef.Modularity;
using Prism.Modularity;

namespace Designer.Services
{
    [Export(typeof(NotificationService))]
    public class NotificationService : NotificationServiceBase<NotificationBase>, IModule
    {
        public void Initialize()
        {
            
        }
    }
}
