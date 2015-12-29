using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Designer.Components;

namespace Designer.Services
{
    [Export(typeof(NotificationService))]
    public class NotificationService : NotificationServiceBase<NotificationBase>
    {

    }
}
