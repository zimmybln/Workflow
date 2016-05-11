using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Designer.Dialogs;
using Designer.Services;
using Prism.Mef;
using Prism.Modularity;
using Prism.Unity;

namespace Designer.Components
{
    public class Bootstrapper : MefBootstrapper   
    {
        protected override DependencyObject CreateShell()
        {
            return this.Container.GetExportedValue<MainWindow>();
        }

        protected override void ConfigureAggregateCatalog()
        {
            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(Bootstrapper).Assembly));
        }

        protected override void InitializeModules()
        {
            base.InitializeModules();

            ModuleCatalog moduleCatalog = (ModuleCatalog) this.ModuleCatalog;

            moduleCatalog.AddModule(typeof (NotificationService));

            

        }

        protected override void InitializeShell()
        {
            base.InitializeShell();

            //Window1 wnd = new Window1();
            //wnd.ShowDialog();

            App.Current.MainWindow = (MainWindow)this.Shell;
            App.Current.MainWindow.Show();
        }

        protected override void ConfigureModuleCatalog()
        {
            base.ConfigureModuleCatalog();

           // this.ModuleCatalog.AddModule(null);
        }


    }
}
