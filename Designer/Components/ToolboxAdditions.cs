﻿using System;
using System.Activities.Presentation.Toolbox;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Designer.Components
{
    public class ToolboxItemDescriptor
    {

        public ToolboxItemDescriptor(string GroupName, Type Type)
        {
            this.Group = GroupName;
            this.ActivityType = Type;
        }

        public string Name { get; set; }

        public string Group { get; private set; }

        public Type ActivityType { get; private set; }
    }

    public class ToolboxItemDescriptorCollection : ObservableCollection<ToolboxItemDescriptor>
    {
        internal ToolboxControl Control { get; set; } = null;

    }

    /// <summary>
    /// This class allow to use the activity toolbox with dynamic content.
    /// </summary>
    public static class ToolboxAdditions
    {
        public static readonly DependencyProperty ToolboxItemsProperty =
            DependencyProperty.RegisterAttached("ToolboxItems", typeof(ToolboxItemDescriptorCollection),
                                                        typeof(ToolboxAdditions),
                                                        new PropertyMetadata(default(ToolboxItemDescriptorCollection), PropertyChangedHandler));


        public static void SetToolboxItems(DependencyObject o, object value)
        {
            o.SetValue(ToolboxItemsProperty, value);
        }

        public static object GetToolboxItems(DependencyObject o)
        {
            return o.GetValue(ToolboxItemsProperty) as ToolboxItemDescriptorCollection;
        }

        private static void PropertyChangedHandler(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            UIElement element = sender as UIElement;
            if (element == null) { return; }

            var ctl = element as ToolboxControl;
            var lst = e.NewValue as ToolboxItemDescriptorCollection;
           
            if (lst != null)
            {

                // the init state is organized by groups
                var groups = from tbitem in lst
                             group tbitem by tbitem.Group
                             into g
                             orderby g.Key
                             select g;

            
                foreach (var group in groups)
                {
                    string strGroup = group.Key;

                    ToolboxCategory category = new ToolboxCategory(strGroup);

                    var items = from tbitem in lst where tbitem.Group == strGroup orderby tbitem.ActivityType.Name select tbitem;

                    foreach (var item in items)
                    {
                        ToolboxItemWrapper tbItemWrapper = null;

                        if (item.ActivityType != null)
                            tbItemWrapper = new ToolboxItemWrapper(item.ActivityType);

                        if (tbItemWrapper != null)
                            category.Add(tbItemWrapper);
                    }

                    ctl.Categories.Add(category);

                }

                lst.Control = ctl;
                lst.CollectionChanged += OnCollectionChanged;

            }


        }

        private static void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            var lst = sender as ToolboxItemDescriptorCollection;

            if (lst?.Control == null)
                return;

            var ctl = lst.Control;

            // looking for category
            foreach (ToolboxItemDescriptor item in args.NewItems)
            {
                var category = ctl.Categories.FirstOrDefault(c => c.CategoryName.Equals(item.Group));

                if (category == null)
                {
                    category = new ToolboxCategory(item.Group);
                    ctl.Categories.Add(category);
                }

                category.Add(new ToolboxItemWrapper(item.ActivityType));
                
            }

            Debug.WriteLine("Ein Element wurde hinzugefügt");
        }
    }
}