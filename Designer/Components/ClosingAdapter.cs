using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Designer.Components
{
    public class ClosingAdapter
    {
        private readonly Action<CancelEventArgs> _actionClosing;

        public ClosingAdapter(Action<CancelEventArgs> LoadedAction)
        {
            _actionClosing = LoadedAction;
        }

        internal void OnClosingHandler(object Sender, CancelEventArgs Args)
        {
            if (_actionClosing != null)
                _actionClosing(Args);
            else
                this.Closing(Args);
        }

        protected virtual void Closing(CancelEventArgs args)
        {

        }

    }
}
