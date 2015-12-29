using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace Designer.Components
{

    public partial class MethodCommand : MethodCommand<object>
    {

        public MethodCommand(Action<ICommand, object> ExecuteMethod)
            : this(ExecuteMethod, null)
        {
        }

        public MethodCommand(Action<ICommand, object> ExecuteMethod, Func<ICommand, object, bool> CanExecuteMethod) :
            base(ExecuteMethod, CanExecuteMethod)
        {
        }
    }

    public partial class MethodCommand<T> : ICommand
         where T : class
    {
        private readonly Action<ICommand, T> _cbExecuteMethod = null;
        private readonly Func<ICommand, T, bool> _cbCanExecuteMethod = null;
        private readonly Dispatcher _hostdispatcher = null;

        public MethodCommand(Action<ICommand, T> ExecuteMethod)
            : this(ExecuteMethod, null)
        {

        }

        public MethodCommand(Action<ICommand, T> ExecuteMethod, Func<ICommand, T, bool> CanExecuteMethod)
        {
            if (ExecuteMethod == null && CanExecuteMethod == null)
                throw new ArgumentNullException("ExecuteMethod");

            this._cbExecuteMethod = ExecuteMethod;
            this._cbCanExecuteMethod = CanExecuteMethod;

            _hostdispatcher = Dispatcher.CurrentDispatcher;
        }

        #region Implementierung der Schnittstelle ICommand

        bool ICommand.CanExecute(object Parameter)
        {
            if (_cbCanExecuteMethod == null) return true;
            return _cbCanExecuteMethod(this, Parameter as T);
        }

        public event EventHandler CanExecuteChanged;

        void ICommand.Execute(object Parameter)
        {
            if (_cbExecuteMethod == null) return;

            var objParameter = Parameter as T;

            _cbExecuteMethod(this, objParameter);
        }

        #endregion

        /// <summary>
        /// Löst das Ereignis aus, das darüber informiert, dass sich der Ausführenstatus geändert hat.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            if (_hostdispatcher.CheckAccess() == false)
                _hostdispatcher.BeginInvoke(new Action(OnCanExecuteChanged), DispatcherPriority.Normal);
            else
                OnCanExecuteChanged();
        }

        private void OnCanExecuteChanged()
        {
            EventHandler evt = this.CanExecuteChanged;

            if (evt != null)
                evt(this, EventArgs.Empty);
        }

        public ICommand Command
        {
            get { return this; }
        }

    }
}
