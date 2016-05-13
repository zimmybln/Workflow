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
using Microsoft.Win32;

namespace BuildActivities.Designs
{
    /// <summary>
    /// Interaktionslogik für BuildDesignerOptionsControl.xaml
    /// </summary>
    public partial class BuildDesignerOptionsControl : UserControl
    {
        public BuildDesignerOptionsControl()
        {
            InitializeComponent();
        }

        private void OnSelectFileClick(object sender, RoutedEventArgs e)
        {
            var dlgSelectFile = new OpenFileDialog();

            if (!String.IsNullOrEmpty(txtExecutable.Text))
            {
                dlgSelectFile.InitialDirectory = System.IO.Path.GetDirectoryName(txtExecutable.Text);
                dlgSelectFile.FileName = System.IO.Path.GetFileName(txtExecutable.Text);
            }

            dlgSelectFile.Multiselect = false;
            dlgSelectFile.ShowReadOnly = false;
            dlgSelectFile.CheckFileExists = true;
            dlgSelectFile.Filter = "Microsoft Build|msbuild.exe";
            dlgSelectFile.Title = "Select MS Build";

            if (dlgSelectFile.ShowDialog() == true)
            {
                txtExecutable.Text = dlgSelectFile.FileName;

                // update binding manually
                BindingOperations.GetBindingExpression(txtExecutable, TextBox.TextProperty)?.UpdateSource();
            }
        }
    }
}
