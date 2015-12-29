using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Designer.Components;

namespace Designer
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            ApplicationServices.BeginRegister();
            ApplicationServices.RegisterModule(this.GetType().Assembly);
            ApplicationServices.EndRegister();

            var mainWindow = new MainWindow();

            mainWindow.Show();
        }
    }
}
