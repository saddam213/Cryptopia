namespace SpriteBuilderService
{
	using System.ComponentModel;
	using System.Configuration.Install;
	using System.ServiceProcess;

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
			serviceInstaller.DisplayName = "Cryptopia.SpriteBuilder";
			serviceInstaller.StartType = ServiceStartMode.Manual;
			serviceInstaller.Description = "Service for building sprite sheets";
			//# This must be identical to the WindowsService.ServiceBase name
			//# set in the constructor of WindowsService.cs
			serviceInstaller.ServiceName = "Cryptopia.SpriteBuilderService";
			this.Installers.Add(serviceProcessInstaller);
			this.Installers.Add(serviceInstaller);
		}
	}
}