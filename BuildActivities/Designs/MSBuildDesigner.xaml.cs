using System;
using System.Activities;
using System.Activities.Expressions;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging; 
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace BuildActivities.Designs
{
    /// <summary>
    /// Dieser Designer stellt die Oberfläche für die 
    /// Aktivität MSBuild zur Verfügung.
    /// </summary>
    public partial class MSBuildDesigner
    {
        public static DependencyProperty OutputDirectoryInfoProperty =
            DependencyProperty.Register("OutputDirectoryInfo", typeof (string), typeof (MSBuildDesigner), new PropertyMetadata("[ohne Angabe]"));

        public static DependencyProperty ProjectFileInfoProperty = 
            DependencyProperty.Register("ProjectFileInfo", typeof(string), typeof(MSBuildDesigner), new PropertyMetadata("[ohne Angabe]"));

        public MSBuildDesigner()
        {
            InitializeComponent();


        }

        public string OutputDirectoryInfo
        {
            get { return GetValue(OutputDirectoryInfoProperty) as string; }
            set { SetValue(OutputDirectoryInfoProperty, value);}
        }

        public string ProjectFileInfo
        {
            get { return GetValue(ProjectFileInfoProperty) as string; }
            set { SetValue(ProjectFileInfoProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (ModelItem != null)
            {
                ModelItem.PropertyChanged += OnPropertyChanged;
                SetPropertyValue();
            }
        }

        private void OnPropertyChanged(object Sender, PropertyChangedEventArgs ChangedEventArgs)
        {
            SetPropertyValue();
        }

        private void SetPropertyValue()
        {
            // Eigenschaft OutputDirectory
            var propertyOutputDirectory = ModelItem?.Properties.FirstOrDefault(x => x.Name == "OutputDirectory");

            var currentOutputDirectory = propertyOutputDirectory?.Value?.GetCurrentValue() as InArgument<String>;

            if (currentOutputDirectory != null)
            {
                OutputDirectoryInfo = (currentOutputDirectory.Expression as Literal<String>)?.Value;
            }

            // Eigenschaft Projektdatei
            var propertyProjectFile = ModelItem?.Properties.FirstOrDefault(x => x.Name == "ProjectFile");

            var currentProjectFile = propertyProjectFile?.Value?.GetCurrentValue() as InArgument<String>;

            var projectfile = (currentProjectFile?.Expression as Literal<String>)?.Value;

            if (!String.IsNullOrWhiteSpace(projectfile))
            {
                ProjectFileInfo = Path.GetFileName(projectfile);
            }
        }
    }
}
