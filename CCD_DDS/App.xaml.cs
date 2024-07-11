using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using USBHID;

namespace CCD_DDS
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
        }
    }
}

/*using Prism;
using Prism.Ioc;
using System.ComponentModel;
using System.Windows;

namespace CCD_DDS
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<HomePage>();
            containerRegistry.RegisterForNavigation<CalibrationPage>();
            containerRegistry.RegisterForNavigation<PrecisionPage>();
            containerRegistry.RegisterForNavigation<SetupPage>();

            containerRegistry.Register<CalibrationStartPageViewModel>();
        }
    }
}*/
