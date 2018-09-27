using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace Cryptopia.OutboundService
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
            serviceInstaller.DisplayName = "Cryptopia.WalletOutboundService";
            serviceInstaller.StartType = ServiceStartMode.Automatic;
            serviceInstaller.Description = "Cryptopia.WalletOutboundService";
            //# This must be identical to the WindowsService.ServiceBase name
            //# set in the constructor of WindowsService.cs
            serviceInstaller.ServiceName = "Cryptopia.WalletOutboundService";
            this.Installers.Add(serviceProcessInstaller);
            this.Installers.Add(serviceInstaller);
        }
    }
}