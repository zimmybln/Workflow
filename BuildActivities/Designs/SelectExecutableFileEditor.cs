using System;
using System.Activities;
using System.Activities.Presentation.PropertyEditing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using Label = System.Windows.Forms.Label;

namespace BuildActivities.Designs
{
    public class SelectExecutableFileEditor : SelectFileEditor
    {
        public SelectExecutableFileEditor() : base("SelectFileEditorTemplate")
        {

        }

        protected override string FileName => "msbuild.exe";

        protected override string DialogTitle => "Select MSBuild";

        protected override string DefaultExtension => "exe";

        protected override string Filter => "MSBuild (*.exe)|msbuild.exe";

        protected override void BeforeFileOk(object sender, CancelEventArgs args)
        {
            
        }

        protected override void OnFileOk(string fileName, PropertyValue propertyValue, IInputElement commandSource)
        {
            propertyValue.Value = new InArgument<string>(fileName);
        }
    }
}
