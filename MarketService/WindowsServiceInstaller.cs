using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace Cryptopia.PoolService
{
    [RunInstaller(true)]
    public class WindowsServiceInstaller : Installer
    {
        /// <summary>
        /// Public Constructor for WindowsServiceInstaller.
        /// - Put all of your Initialization code here.
        /// </summary>
        public WindowsServiceInstaller()
        {
            var serviceProcessInstaller = new ServiceProcessInstaller();
            var serviceInstaller = new ServiceInstaller();

            //# Service Account Information
            serviceProcessInstaller.Account = ServiceAccount.LocalService;
          
            //# Service Information
            serviceInstaller.DisplayName = "Cryptopia.MarketService";
            serviceInstaller.StartType = ServiceStartMode.Manual;
            serviceInstaller.Description = "Cryptopia.MarketService";
            //# This must be identical to the WindowsService.ServiceBase name
            //# set in the constructor of WindowsService.cs
            serviceInstaller.ServiceName = "Cryptopia.MarketService";
            this.Installers.Add(serviceProcessInstaller);
            this.Installers.Add(serviceInstaller);
        }
    }
}