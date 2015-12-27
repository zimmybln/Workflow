using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Designer.Components
{
    internal abstract class ExecuteActionBase<T>
         where T : EventArgs
    {
        private readonly RoutedEvent mv_routedEvent;
        protected DependencyProperty mv_property;

        protected ExecuteActionBase(RoutedEvent Event)
        {
            mv_routedEvent = Event;
        }

        protected abstract Delegate GetHandler();

        /// <summary>
        /// Listens for a change in the DependencyProperty that we are assigned to, and
        /// adjusts the EventHandlers accordingly
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void PropertyChangedHandler(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            // the first time the property changes,
            // make a note of which property we are supposed
            // to be watching
            if (mv_property == null)
                mv_property = e.Property;

            UIElement element = sender as UIElement;
            if (element == null) { return; }

            if (e.OldValue != null)
                element.RemoveHandler(mv_routedEvent, this.GetHandler());

            if (e.NewValue != null)
                element.AddHandler(mv_routedEvent, this.GetHandler());
        }


        protected virtual void HandleEvent(object sender, T e)
        {
            var dp = sender as DependencyObject;
            if (dp == null)
                return;

            // Ermitteln der Aktion aus dem Objekt
            var a = dp.GetValue(mv_property) as ActionAdapter<T>;

            // Wenn keine zugeordnete Aktion gefunden werden konnte,
            // ist hier Schluss
            if (a == null)
                return;

            // Ausführen der Aktion
            a.Action(sender, e);
        }
    }
}
