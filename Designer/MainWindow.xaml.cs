using System;
using System.Activities;
using System.Activities.Presentation;
using System.Activities.Presentation.Converters;
using System.Activities.Presentation.Model;
using System.Activities.Statements;
using System.Activities.XamlIntegration;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xaml;
using Designer.Models;
using Fluent;

namespace Designer
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    [Export]
    public partial class MainWindow : RibbonWindow
    {
        private Point _startpoint;


        public MainWindow()
        {
            InitializeComponent();
        }

        [Import]
        public MainWindowModel ViewModel
        {
            set { this.DataContext = value; }
        }

        private void LstTemplates_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _startpoint = e.GetPosition(null);
        }


        private void LstTemplates_OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            // Get the current mouse position
            Point mousePos = e.GetPosition(null);
            Vector diff = _startpoint - mousePos;

            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                Debug.WriteLine("drag " + e.LeftButton.ToString());
                // Get the dragged ListViewItem
                // Get the dragged ListViewItem
                ListView listView = sender as ListView;
                ListViewItem listViewItem =
                    FindAnchestor<ListViewItem>((DependencyObject)e.OriginalSource);

                if (listView == null || listViewItem == null)
                    return;

                // Find the data behind the ListViewItem
                string strTemplate = (string)listView.ItemContainerGenerator.
                    ItemFromContainer(listViewItem);

                var item = new Sequence();


                // Hier wird der Verweis auf eine Aktivität übertragen
                //DataObject dragData = new DataObject(DragDropHelper.WorkflowItemTypeNameFormat, typeof(If).AssemblyQualifiedName);
                //DragDrop.DoDragDrop(listViewItem, dragData, DragDropEffects.Move);




                // DataObject dragData = new DataObject(DragDropHelper.ModelItemDataFormat, item);

                //IActivityToolboxService svr =
                //    WorkflowEditor._coreDesigner.Context.Services.GetService<IActivityToolboxService>();

                //if (svr != null)
                //{

                //}

                var tree = WorkflowEditor._coreDesigner.Context.Services.GetService<ModelTreeManager>();

                
                ModelItem mi = tree.CreateModelItem(null, item);

                

                WorkflowViewElement view = new WorkflowViewElement();

                view.ModelItem = mi;
                
                DataObject data = new DataObject(DragDropHelper.ModelItemDataFormat, mi);

                DragDrop.DoDragDrop(listViewItem, data, DragDropEffects.Copy);

            }
        }

        private static T FindAnchestor<T>(DependencyObject current)
    where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }
    }

}
