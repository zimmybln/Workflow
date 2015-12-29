using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Designer.Components
{
    public abstract class ServiceBase : PropertyContainer, IService
    {
        private readonly List<WeakReference> _lstPropertyChangedListeners;

        protected ServiceBase()
        {
            _lstPropertyChangedListeners = new List<WeakReference>();
            Dispatcher = System.Windows.Application.Current.Dispatcher;
        }

        protected override void RaisePropertyChanged(string PropertyName)
        {
            
        }

        #region Eigenschaften

        /// <summary>
        /// Liefert den Dispatcher, in dem der Dienst erstellt worden ist.
        /// </summary>
        protected Dispatcher Dispatcher { get; }

        #endregion
    }
}
