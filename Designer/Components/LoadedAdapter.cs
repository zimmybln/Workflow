using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Designer.Components
{
    public class LoadedAdapter
    {
        private readonly Action _actionLoaded;
        private readonly ICommand _cmdAction;

        public LoadedAdapter()
        {

        }

        public LoadedAdapter(ICommand Command)
        {
            _cmdAction = Command;
        }

        public void RaiseLoaded()
        {
            OnLoadedHandler(this, new RoutedEventArgs());
        }

        public LoadedAdapter(Action LoadedAction)
        {
            _actionLoaded = LoadedAction;
        }

        internal void OnLoadedHandler(object Sender, RoutedEventArgs Args)
        {
            if (_actionLoaded != null)
                _actionLoaded();
            else if (_cmdAction != null)
                _cmdAction.Execute(null);
            else
                this.Loaded();
        }

        protected virtual void Loaded()
        {

        }
    }
}
