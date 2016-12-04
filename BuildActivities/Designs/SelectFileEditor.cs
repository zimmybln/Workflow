using System;
using System.Activities.Presentation.PropertyEditing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace BuildActivities.Designs
{
    public abstract class SelectFileEditor : DialogPropertyValueEditor
    {

        private DialogResources res = new DialogResources();

        protected SelectFileEditor(string ressourceKey)
        {
            if (String.IsNullOrEmpty(ressourceKey))
            {
                throw new ArgumentNullException(nameof(ressourceKey));
            }

            this.InlineEditorTemplate = res[ressourceKey] as DataTemplate;
        }

        public override void ShowDialog(PropertyValue propertyValue, IInputElement commandSource)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            var filename = FileName;
            if (!String.IsNullOrWhiteSpace(filename))
            {
                dlg.FileName = filename;
            }

            var dialogtitle = DialogTitle;
            if (!String.IsNullOrWhiteSpace(dialogtitle))
            {
                dlg.Title = dialogtitle;
            }

            var filter = Filter;
            if (!String.IsNullOrWhiteSpace(filter))
            {
                dlg.Filter = filter;
            }

            dlg.FilterIndex = 0;
            dlg.Multiselect = false;
            dlg.FileOk += this.BeforeFileOk;
            

            if (dlg.ShowDialog() == true)
            {
                this.OnFileOk(dlg.FileName, propertyValue, commandSource);
            }
        }

        protected abstract string FileName { get; }

        protected abstract string DialogTitle { get; }

        protected abstract string DefaultExtension { get; }

        protected abstract string Filter { get; }

        protected virtual void BeforeFileOk(object sender, CancelEventArgs args)
        {
            
        }

        protected virtual void OnFileOk(string fileName, PropertyValue propertyValue, IInputElement commandSource)
        {
            
        }
    }
}
