using System;
using System.Activities;
using System.Activities.Presentation.PropertyEditing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace BuildActivities.Designs
{
    public class SelectDirectoryEditor : DialogPropertyValueEditor
    {
        private Dialogs res = new Dialogs();

        public SelectDirectoryEditor()
        {
            this.InlineEditorTemplate = res["SelectFileEditorTemplate"] as DataTemplate;
        }

        public override void ShowDialog(PropertyValue propertyValue, IInputElement commandSource)
        {
            var dlg = new FolderBrowserDialog();

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
