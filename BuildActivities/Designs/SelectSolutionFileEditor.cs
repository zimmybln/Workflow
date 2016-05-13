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

namespace BuildActivities.Designs
{
    public class SelectSolutionFileEditor : SelectFileEditor
    {
        public SelectSolutionFileEditor() : base("SelectFileEditorTemplate")
        {

        }

        protected override string FileName => "";

        protected override string DialogTitle => "Select Solution or Project";

        protected override string DefaultExtension => "";

        protected override string Filter => "Solution or Project (*.sln)|*.sln;*.csproj";

        protected override void BeforeFileOk(object sender, CancelEventArgs args)
        {

        }

        protected override void OnFileOk(string fileName, PropertyValue propertyValue, IInputElement commandSource)
        {
            propertyValue.StringValue = fileName;
            //propertyValue.Value = new InArgument<string>(fileName);
        }
    }
}
