using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Designer.Components
{
    public class UnloadedAdapter
    {
        private readonly Action _actionUnloaded;

        public UnloadedAdapter(Action action)
        {
            _actionUnloaded = action;
        }

        internal void OnUnloadedHandler(object Sender, RoutedEventArgs Args)
        {
            if (_actionUnloaded != null)
                _actionUnloaded();
            //else if (_cmdAction != null)
            //    _cmdAction.Execute(null);
            //else
            //    this.Loaded();
        }
    }
}
