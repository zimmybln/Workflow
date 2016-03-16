using System;
using System.Collections.Generic;
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
using System.Activities;
using System.Collections.ObjectModel;
using System.Activities.Presentation.Validation;
using System.Activities.Core.Presentation;
using System.Activities.Presentation;
using System.Activities.Presentation.Model;
using System.Activities.Presentation.View;
using System.Diagnostics;
using System.Runtime.Versioning;
using Designer.Controls.Services;

namespace Designer.Controls
{
    /// <summary>
    /// Interaktionslogik für WorkflowEditor.xaml
    /// </summary>
    public partial class WorkflowEditor : UserControl
    {
        public WorkflowEditor()
        {
            InitializeComponent();
            
            // Container für Benachrichtigungen anlegen
            Messages = new ObservableCollection<EditorMessage>();


            var dm = new DesignerMetadata();
            dm.Register();

            InitializeWorkflow();
        }

        public static readonly DependencyProperty SelectedWorkflowProperty = DependencyProperty.Register("SelectedWorkflow", typeof(Activity),
                                                                                  typeof(WorkflowEditor),
                                                                                  new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                                                                                      OnWorkflowPropertyChanged));

        public static readonly DependencyProperty PropertyViewProperty = DependencyProperty.Register("PropertyView", typeof(UIElement), typeof(WorkflowEditor),
                                                                                                new PropertyMetadata(null));

        public static readonly DependencyProperty OutlineViewProperty = DependencyProperty.Register("OutlineView", typeof(UIElement), typeof(WorkflowEditor),
                                                                                                new PropertyMetadata(null));

        public static readonly DependencyProperty ChangedProperty = DependencyProperty.Register("Changed", typeof(bool), typeof(WorkflowEditor),
                                                                                                new PropertyMetadata(false));

        public WorkflowDesigner _coreDesigner;
        private readonly List<ModelItem> _currentSelectedItems = new List<ModelItem>();

        public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent("SelectionChanged",
            RoutingStrategy.Bubble, typeof (SelectionChangedEventHandler), typeof (WorkflowEditor));

        public event SelectionChangedEventHandler SelectionChanged
        {
            add { AddHandler(SelectionChangedEvent, value);}
            remove { RemoveHandler(SelectionChangedEvent, value);}
        }
        
        /// <summary>
        ///  Initialisiert eine neue Instanz des Workflowdesigners
        /// </summary>
        private void InitializeWorkflow()
        {

            // clean up the previous instance
            if (_coreDesigner != null)
            {
                _coreDesigner.ModelChanged -= WorkflowDesignerOnModelChanged;
                grdDesignerHost.Children.Clear();
            }

            // create and configure the designer control
            _coreDesigner = new WorkflowDesigner();

            // apply the services / configurations
            _coreDesigner.Context.Services.Publish(typeof(IValidationErrorService), new ValidationErrorService(this.Messages));
            _coreDesigner.Context.Items.Subscribe(typeof(Selection), OnSelectionChanged);
            

            grdDesignerHost.Children.Add(_coreDesigner.View);

            PropertyView = _coreDesigner.PropertyInspectorView;
            OutlineView = _coreDesigner.OutlineView;
            
            _coreDesigner.ModelChanged += WorkflowDesignerOnModelChanged;

            // Mögliche Dienste:
            // - DesignerConfigurationService

            var configurationService =
                _coreDesigner.Context.Services.GetRequiredService<DesignerConfigurationService>();
            
            if (configurationService != null)
            {
                configurationService.AutoConnectEnabled = true;
                configurationService.AutoSplitEnabled = true;
                configurationService.AutoSurroundWithSequenceEnabled = true;
                configurationService.BackgroundValidationEnabled = true;
                configurationService.MultipleItemsContextMenuEnabled = true;
                configurationService.MultipleItemsDragDropEnabled = true;
                configurationService.NamespaceConversionEnabled = false;
                configurationService.PanModeEnabled = true;
                configurationService.LoadingFromUntrustedSourceEnabled = false;

                configurationService.AnnotationEnabled = true;
                configurationService.RubberBandSelectionEnabled = true;
                configurationService.TargetFrameworkName = new FrameworkName(".NETFramework,Version=v4.5");
            }

        }

        private void OnSelectionChanged(ContextItem item)
        {
            var selection = item as Selection;

            if (selection?.PrimarySelection == null)
                return;

            var addedItems = selection.SelectedObjects.ToList();
            var args = new SelectionChangedEventArgs(SelectionChangedEvent, _currentSelectedItems, addedItems);
            RaiseEvent(args);

            // keep the current selected items
            _currentSelectedItems.Clear();
            _currentSelectedItems.AddRange(addedItems);
        }

        private void WorkflowDesignerOnModelChanged(object Sender, EventArgs Args)
        {
            Changed = true;
        }

        #region Eigenschaften

        public ObservableCollection<EditorMessage> Messages { get; private set; }

        public Activity SelectedWorkflow
        {
            get { return GetValue(SelectedWorkflowProperty) as Activity; }
            set { SetValue(SelectedWorkflowProperty, value); }
        }

        public UIElement PropertyView
        {
            get { return GetValue(PropertyViewProperty) as UIElement; }
            set { SetValue(PropertyViewProperty, value); }
        }

        public UIElement OutlineView
        {
            get { return GetValue(OutlineViewProperty) as UIElement; }
            set { SetValue(OutlineViewProperty, value); }
        }

        public bool Changed
        {
            get { return (bool)GetValue(ChangedProperty); }
            set { SetValue(ChangedProperty, value); }
        }

        #endregion

        private static void OnWorkflowPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var wd = d as WorkflowEditor;
            var sq = e.NewValue as Activity;

            if (sq != null)
            {
                wd.InitializeWorkflow();

                wd._coreDesigner.Load(sq);

                var validationservice = wd._coreDesigner.Context.Services.GetService<ValidationService>();

                validationservice?.ValidateWorkflow();
            }
        }

        private void GrdDesignerHost_OnDragEnter(object sender, DragEventArgs e)
        {

        }

        private void GrdDesignerHost_OnDrop(object sender, DragEventArgs e)
        {
            var data = e.Data.GetData(DragDropHelper.WorkflowItemTypeNameFormat);


        }
    }
}
