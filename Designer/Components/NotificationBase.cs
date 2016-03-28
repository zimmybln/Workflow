using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Designer.Components
{
    public interface INotificationMessage
    {
        string Message { get; }
    }


    public class NotificationBase : INotification
    {

    }

    public class ExecutionFinishedNotification : NotificationBase, INotificationMessage
    {
        public ExecutionFinishedNotification(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }

    public class ResetMessageNotification : NotificationBase, INotificationMessage
    {
        public ResetMessageNotification()
        {
            Message = String.Empty;
        }

        public string Message { get; }
    }
}
