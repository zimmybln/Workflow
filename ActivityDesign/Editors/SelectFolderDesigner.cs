using System;
using System.Activities.Presentation.PropertyEditing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ActivityDesign.Templates;
using Label = System.Reflection.Emit.Label;

namespace ActivityDesign.Editors
{
    public class SelectFolderDesigner : DialogPropertyValueEditor
    {

        private Dialogs res = new Dialogs();

        public SelectFolderDesigner()
        {
            this.InlineEditorTemplate = new DataTemplate();

            var stack = new FrameworkElementFactory(typeof(DockPanel));
            stack.SetValue(DockPanel.LastChildFillProperty, true);

            var editModeSwitch = new FrameworkElementFactory(typeof(EditModeSwitchButton));

            editModeSwitch.SetValue(EditModeSwitchButton.TargetEditModeProperty, PropertyContainerEditMode.Dialog);
            editModeSwitch.SetValue(DockPanel.DockProperty, Dock.Right);

            stack.AppendChild(editModeSwitch);


            // Erstellen und Konfiguration des Labels
            var label = new FrameworkElementFactory(typeof(Label));
            //Binding labelBinding = new Binding("Value"); 

            // Setzen der Eigenschaften des Labels
            label.SetValue(ContentControl.ContentProperty, "Bitte den Dialog öffnen");
            label.SetValue(DockPanel.DockProperty, Dock.Left);

            stack.AppendChild(label);

            this.InlineEditorTemplate.VisualTree = stack;

            // Zuordnen des DataTemplates
            // this.InlineEditorTemplate = res["SelectFileEditorTemplate"] as DataTemplate;
        }

        public override void ShowDialog(PropertyValue propertyValue, IInputElement commandSource)
        {
            MessageBox.Show("Dialog");
        }
    }
}
