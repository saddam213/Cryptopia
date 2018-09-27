using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace AdmintopiaService
{
	[RunInstaller(true)]
	public class WindowsServiceInstaller : Installer
	{
		public WindowsServiceInstaller()
		{
			var serviceProcessInstaller = new ServiceProcessInstaller();
			var serviceInstaller = new ServiceInstaller();

			//# Service Account Information
			serviceProcessInstaller.Account = ServiceAccount.LocalService;

			//# Service Information
			serviceInstaller.DisplayName = "Cryptopia.Admintopia";
			serviceInstaller.StartType = ServiceStartMode.Automatic;
			serviceInstaller.Description = "Service for communicating with wallets for transaction related tasks";
			//# This must be identical to the WindowsService.ServiceBase name
			//# set in the constructor of WindowsService.cs
			serviceInstaller.ServiceName = "Cryptopia.AdmintopiaService";
			this.Installers.Add(serviceProcessInstaller);
			this.Installers.Add(serviceInstaller);
		}
	}
}