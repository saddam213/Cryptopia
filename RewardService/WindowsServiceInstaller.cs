using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace Cryptopia.RewardService
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
            serviceInstaller.DisplayName = "Cryptopia.RewardService";
            serviceInstaller.StartType = ServiceStartMode.Manual;
			serviceInstaller.Description = "Cryptopia.RewardService";
            //# This must be identical to the WindowsService.ServiceBase name
            //# set in the constructor of WindowsService.cs
			serviceInstaller.ServiceName = "Cryptopia.RewardService";
            this.Installers.Add(serviceProcessInstaller);
            this.Installers.Add(serviceInstaller);
        }
    }
}