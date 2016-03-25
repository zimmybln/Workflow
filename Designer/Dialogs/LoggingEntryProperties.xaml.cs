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
using System.Windows.Shapes;
using Designer.Components;
using Designer.Models;

namespace Designer.Dialogs
{
    /// <summary>
    /// Interaktionslogik für LoggingEntryProperties.xaml
    /// </summary>
    public partial class LoggingEntryProperties : Window
    {
        public LoggingEntryProperties(LoggingEntry entry)
        {
            InitializeComponent();

            CloseCommand = new MethodCommand((command, o) => this.Close());

            Entry = entry;

            this.DataContext = this;
        }

        public ICommand CloseCommand { get; }

        public LoggingEntry Entry { get; }


    }
}
