using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;
using System.Configuration;

namespace Cryptopia.DepositTrackerService
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
            var walletHostName = ConfigurationManager.AppSettings["WalletHostName"];
            serviceInstaller.DisplayName = $"Cryptopia Deposit Tracker Service - wallet: {walletHostName}";
            serviceInstaller.StartType = ServiceStartMode.Manual;
            serviceInstaller.Description = "Service for tracking wallet deposits.";
            //# This must be identical to the WindowsService.ServiceBase name
            //# set in the constructor of WindowsService.cs
            serviceInstaller.ServiceName = $"Cryptopia.DepositTrackerService.{walletHostName}";
            this.Installers.Add(serviceProcessInstaller);
            this.Installers.Add(serviceInstaller);
        }
    }
}