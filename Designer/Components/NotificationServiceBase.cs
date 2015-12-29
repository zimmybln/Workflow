using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Designer.Components
{
    public class NotificationServiceBase<T> : ServiceBase, INotificationService<T>
         where T : INotification
    {

        private readonly List<GenericWeakEventSubscription<T>> mv_lstListenersForNotification;

        public delegate void NotificationHandler(object sender, T Notification);

        /// <summary>
        /// Benachrichtigt über das Verschicken von Nachrichten
        /// </summary>
        public event NotificationHandler OnNotify
        {
            add
            {
                mv_lstListenersForNotification.Add(new GenericWeakEventSubscription<T>(value.Target, value.Method));
            }
            remove
            {
                GenericWeakEventSubscription<T> item = mv_lstListenersForNotification.FirstOrDefault(g => g.Reference.IsAlive && g.Reference != null && g.Reference.Target == value.Target);

                if (item != null)
                    mv_lstListenersForNotification.Remove(item);
            }
        }

        public NotificationServiceBase()
        {
            mv_lstListenersForNotification = new List<GenericWeakEventSubscription<T>>();
            SendNotificationsToUI = true;
        }

        /// <summary>
        /// Verschickt eine Nachricht an alle im Delegate registrierten Komponenten. Dabei wird sichergestellt, dass
        /// die Nachrichten im Thread verschickt werden, in dem der Dienst erstellt worden ist.
        /// </summary>
        public void Notify(T Notification)
        {
            if (SendNotificationsToUI && this.Dispatcher != null)
            {
                if (this.Dispatcher.CheckAccess() == false)
                    this.Dispatcher.BeginInvoke(
                        new Action<T>(N => InternalNotify(N)),
                        DispatcherPriority.Normal, Notification);

                else
                    InternalNotify(Notification);
            }
            else
                InternalNotify(Notification);
        }

        /// <summary>
        /// Verteilt die Benachrichtigung an alle Beobachter.
        /// </summary>
        /// <param name="Notification"></param>
        private void InternalNotify(T Notification)
        {
            foreach (GenericWeakEventSubscription<T> l in mv_lstListenersForNotification)
                l.Deliver(this, Notification);

        }

        /// <summary>
        /// Gibt an, ob die Benachrichtigungen im UI Thread der Anwendung verschickt werden
        /// oder im Kontext des Aufrufers
        /// </summary>
        public bool SendNotificationsToUI { get; set; }
    }
}
