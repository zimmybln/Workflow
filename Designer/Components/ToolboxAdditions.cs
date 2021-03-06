﻿using System;
using System.Activities;
using System.Activities.Presentation.Toolbox;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Designer.Components
{
    public class ToolboxItemDescriptor
    {

        public ToolboxItemDescriptor(string groupName, Type type)
        {
            this.Group = groupName;
            this.ActivityType = type;
        }

        public string Name { get; set; }

        public string Group { get; private set; }

        public Type ActivityType { get; private set; }

        
    }

    public class ToolboxItemDescriptorCollection : ObservableCollection<ToolboxItemDescriptor>
    {
        internal ToolboxControl Control { get; set; } = null;
        internal List<string> Directories { get; set; } = new List<string>(); 

        public IEnumerable<Type> AddFromDirectory(string directoryName)
        {
           List<Type> activityTypes = new List<Type>();
            
            // verify if the directory is rooted
            if (!Path.IsPathRooted(directoryName))
            {
                directoryName = Path.Combine(Environment.CurrentDirectory, directoryName);
            }

            if (!Directory.Exists(directoryName))
                return activityTypes;
            
            foreach (string file in Directory.GetFiles(directoryName, "*.dll"))
            {
                Assembly assembly = Assembly.LoadFile(Path.Combine(directoryName, file));

                var types = AddFromAssembly(assembly);

                if (types != null && types.Any())
                {
                    activityTypes.AddRange(types);
                }

                if (!Directories.Contains(directoryName))
                {
                    Directories.Add(directoryName);
                }
            }

            return activityTypes;
        }

        
        public int AddFromFile(string assemblyFileName)
        {
            return 0;
        }

        /// <summary>
        /// Adds all activities from the given assembly
        /// </summary>
        public IEnumerable<Type> AddFromAssembly(Assembly lib)
        {
            List<Type> activityType = new List<Type>();
            Type typeofactivity = typeof (Activity);

            // get the filename
            var filename = new Uri(lib.CodeBase).LocalPath;
            var categoryname = Path.GetFileNameWithoutExtension(filename);

            foreach (Type type in lib.GetExportedTypes().Where(t => t.IsSubclassOf(typeofactivity)))
            {
                if (!type.IsAbstract && type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new Type[0], null) != null)
                {
                    try
                    {
                        this.Add(new ToolboxItemDescriptor(categoryname, type));
                        activityType.Add(type);
                    }
                    catch (Exception)
                    {
                        //throw;
                    }
                    
                }
            }

            return activityType;
        }

        public void AddRange(string categoryName, params Type[] items)
        {
            Type typeofactivity = typeof(Activity);

            foreach (Type type in items.Where(t => t.IsSubclassOf(typeofactivity)))
            {
                if (!type.IsAbstract && type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new Type[0], null) != null)
                {
                    try
                    {
                        this.Add(new ToolboxItemDescriptor(categoryName, type));
                    }
                    catch (Exception)
                    {
                        //throw;
                    }
                }
            }
        }



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

            var resolveHandler = new ResolveEventHandler(delegate(object o, ResolveEventArgs args)
            {
                var filename = args.Name.Split(',')[0];

                foreach (string directory in lst.Directories)
                {
                    var assemblyfile = Path.Combine(directory, filename) + ".dll";


                    if (File.Exists(assemblyfile))
                    {
                        return Assembly.LoadFile(assemblyfile);
                    }
                }

                return null;
            });

            AppDomain.CurrentDomain.AssemblyResolve += resolveHandler;

           
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

                AppDomain.CurrentDomain.AssemblyResolve -= resolveHandler;

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
        }
    }
}
