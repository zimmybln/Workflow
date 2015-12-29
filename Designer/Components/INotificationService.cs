using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Designer.Components
{
    public interface INotificationService<T> : IService
        where T : INotification
    {
        /// <summary>
        /// Benachrichtigt über das Verschicken von Nachrichten
        /// </summary>
        event NotificationServiceBase<T>.NotificationHandler OnNotify;

        /// <summary>
        /// Verschickt eine Nachricht an alle im Delegate registrierten Komponenten.
        /// </summary>
        void Notify(T Notification);

        /// <summary>
        /// Gibt an, ob die Benachrichtigungen im UI Thread der Anwendung verschickt werden
        /// oder im Kontext des Aufrufers
        /// </summary>
        bool SendNotificationsToUI { get; set; }
    }
}
