using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Designer.Components;
using Designer.Properties;

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

            Settings.Default.Reload();
            
            ApplicationServices.BeginRegister();
            ApplicationServices.RegisterModule(this.GetType().Assembly);
            ApplicationServices.EndRegister();

            var mainWindow = new MainWindow();

            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            
            Settings.Default.Save();
        }
    }
}
