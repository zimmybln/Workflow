using System;
using System.Activities;
using System.Activities.Expressions;
using System.Activities.Presentation.PropertyEditing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace BuildActivities.Designs
{
    /// <summary>
    /// Dieser Editor erlaubt die Auswahl eines Verzeichnisses für den
    /// Wert einer Eigenschaft.
    /// </summary>
    public class SelectDirectoryEditor : DialogPropertyValueEditor
    {
        private readonly DialogResources _resources = new DialogResources();

        public SelectDirectoryEditor()
        {
            this.InlineEditorTemplate = _resources["SelectFileEditorTemplate"] as DataTemplate;
        }

        public override void ShowDialog(PropertyValue propertyValue, IInputElement commandSource)
        {
            var dlg = new FolderBrowserDialog();

            var value = propertyValue.Value as InArgument<String>;

            dlg.Description =
                $"Bitte legen Sie den Wert der Eigenschaft {propertyValue.ParentProperty.DisplayName} fest.";

            if (value != null)
            {
                var literal = value.Expression as Literal<String>;
                dlg.SelectedPath = literal?.Value;
            }


            if (dlg.ShowDialog() == DialogResult.OK)
            {
                this.OnPathOk(dlg.SelectedPath, propertyValue, commandSource);
            }
        }

        protected virtual void OnPathOk(string path, PropertyValue propertyValue, IInputElement commandSource)
        {
            propertyValue.Value = new InArgument<string>(path);
        }
    }
}
