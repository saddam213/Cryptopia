using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Microsoft.AspNet.SignalR.Hubs;

namespace Cryptopia.DependencyInjection.Installers
{
	public class SignalrInstaller : IWindsorInstaller
	{
		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			container.Register(Classes.FromAssemblyNamed(DependencyRegistrar.AssemblyName)
				.BasedOn<IHub>()
				.LifestyleTransient()
				);


		}
	}
}