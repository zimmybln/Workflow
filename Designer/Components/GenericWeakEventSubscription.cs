using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Designer.Components
{
    public class GenericWeakEventSubscription<TEventArgs> : IComparable<GenericWeakEventSubscription<TEventArgs>>
    {
        private WeakReference mv_weakReference;
        private MethodInfo mv_objMethod;

        public GenericWeakEventSubscription(object EventHost, MethodInfo Method)
        {
            mv_weakReference = new WeakReference(EventHost);
            mv_objMethod = Method;
        }

        /// <summary>
        /// Übermittelt die Parameter an den Ereignisempfänger, wenn er noch existiert.
        /// </summary>
        public void Deliver(object sender, TEventArgs Args)
        {
            if (mv_weakReference.IsAlive && mv_weakReference.Target != null)
                mv_objMethod.Invoke(mv_weakReference.Target, BindingFlags.Default, null, new object[] { sender, Args }, null);
        }

        public int CompareTo(GenericWeakEventSubscription<TEventArgs> other)
        {
            return object.ReferenceEquals(mv_weakReference, other.mv_weakReference) == true ? 0 : 1;
        }

        public WeakReference Reference
        {
            get
            {
                return mv_weakReference;
            }
        }
    }
}
