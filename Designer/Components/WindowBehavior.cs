using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Designer.Components
{
    class WindowBehavior
    {
        #region Loaded

        public static readonly DependencyProperty LoadedProperty =
                DependencyProperty.RegisterAttached("Loaded", typeof(LoadedAdapter), typeof(WindowBehavior),
                                        new PropertyMetadata(null, LoadedPropertyChangedHandler));

        /// <summary>
        /// Methode zum Setzen der Eigenschaft
        /// </summary>
        public static void SetLoaded(DependencyObject o, object value)
        {
            o.SetValue(LoadedProperty, value);
        }

        /// <summary>
        /// Methode zum Lesen der Eigenschaft
        /// </summary>
        public static object GetLoaded(DependencyObject o)
        {
            return o.GetValue(LoadedProperty) as LoadedAdapter;
        }

        /// <summary>
        /// Diese Methode wird aufgerufen, wenn sich die Zuordnung zur Attached Property ändert.
        /// </summary>
        private static void LoadedPropertyChangedHandler(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            UIElement element = sender as UIElement;
            if (element == null) { return; }

            if (e.OldValue != null)
            {
                var adapter = (LoadedAdapter)e.OldValue;
                element.RemoveHandler(FrameworkElement.LoadedEvent, new RoutedEventHandler(adapter.OnLoadedHandler));
            }

            if (e.NewValue != null)
            {
                var adapter = (LoadedAdapter)e.NewValue;
                element.AddHandler(FrameworkElement.LoadedEvent, new RoutedEventHandler(adapter.OnLoadedHandler));
            }
        }

        #endregion

        #region Unloaded

        public static readonly DependencyProperty UnloadedProperty = DependencyProperty.RegisterAttached("Unloaded", typeof(UnloadedAdapter),
                                        typeof(WindowBehavior), new PropertyMetadata(null, UnloadedPropertyChangedHandler));

        public static void SetUnloaded(DependencyObject o, ActionAdapter<RoutedEventArgs> value)
        {
            o.SetValue(UnloadedProperty, value);
        }

        public static UnloadedAdapter GetUnloaded(DependencyObject o)
        {
            return o.GetValue(UnloadedProperty) as UnloadedAdapter;
        }

        private static void UnloadedPropertyChangedHandler(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            UIElement element = sender as UIElement;
            if (element == null) { return; }

            if (e.OldValue != null)
            {
                var adapter = (UnloadedAdapter)e.OldValue;
                element.RemoveHandler(FrameworkElement.UnloadedEvent, new RoutedEventHandler(adapter.OnUnloadedHandler));
            }

            if (e.NewValue != null)
            {
                var adapter = (UnloadedAdapter)e.NewValue;
                element.AddHandler(FrameworkElement.UnloadedEvent, new RoutedEventHandler(adapter.OnUnloadedHandler));
            }
        }

        private class UnloadedEventBehavior : ExecuteActionBase<RoutedEventArgs>
        {
            public UnloadedEventBehavior(RoutedEvent routedEvent) : base(routedEvent) { }

            protected override Delegate GetHandler()
            {
                return new RoutedEventHandler(HandleEvent);
            }
        }

        #endregion

        #region Closing

        public static readonly DependencyProperty ClosingProperty =
                DependencyProperty.RegisterAttached("Closing", typeof(ClosingAdapter), typeof(WindowBehavior),
                    new PropertyMetadata(null, ClosingPropertyChangedHandler));


        public static void SetClosing(DependencyObject o, object value)
        {
            o.SetValue(ClosingProperty, value);
        }

        public static object GetClosing(DependencyObject o)
        {
            return o.GetValue(ClosingProperty) as ClosingAdapter;
        }

        private static void ClosingPropertyChangedHandler(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Window element = sender as Window;
            if (element == null) { return; }

            if (e.OldValue != null)
            {
                var adapter = (ClosingAdapter)e.OldValue;
                element.Closing -= adapter.OnClosingHandler;
            }

            if (e.NewValue != null)
            {
                var adapter = (ClosingAdapter)e.NewValue;
                element.Closing += adapter.OnClosingHandler;
            }
        }

        #endregion

    }
}
