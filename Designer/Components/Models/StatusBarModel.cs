using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Designer.Services;

namespace Designer.Components
{
    public class StatusBarModel : INotifyPropertyChanged
    {
        private readonly NotificationService _notificationService;
        private Timer _timer;
        private string _text;

        public StatusBarModel(NotificationService notifications)
        {
            _notificationService = notifications;

            _notificationService.OnNotify += OnNotify;
        }

        private void OnNotify(object sender, NotificationBase notification)
        {
            string message;
            if (GetMessage(notification, out message))
            {
                // reset the timer to zero
                _timer?.Change(0, 0);

                Text = message;

                if (_timer == null)
                {
                    _timer = new Timer(OnTimerCallback);
                }
                _timer.Change(5000, 0);
            }
        }

        private void OnTimerCallback(object state)
        {
            var dispatcher = System.Windows.Application.Current.Dispatcher;

            if (!dispatcher.CheckAccess())
            {
                dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)(ResetStatusBar));
            }
            else
            {
                ResetStatusBar();
            }
        }

        protected virtual void ResetStatusBar()
        {
            _notificationService.Notify(new ResetMessageNotification());
        }

        protected virtual bool GetMessage(NotificationBase notification, out string message)
        {
            if (notification is INotificationMessage)
            {
                message = ((INotificationMessage)notification).Message;
                return true;
            }
            message = null;
            return false;
        }

        public string Text
        {
            get { return _text; }
            set
            {
                if (!String.Equals(value, _text))
                {
                    _text = value;
                    RaisePropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
