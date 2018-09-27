using System.Web.Http.Controllers;
using System.Web.Mvc;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Cryptopia.DependencyInjection.Installers
{
	public class ControllersInstaller : IWindsorInstaller
	{
		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			container.Register(Classes.FromAssemblyNamed(DependencyRegistrar.AssemblyName)
				.BasedOn<IController>()
				.LifestyleTransient()
				);

			container.Register(Classes.FromAssemblyNamed(DependencyRegistrar.AssemblyName)
				.BasedOn<IHttpController>()
				.LifestyleTransient()
				);

		}
	}
}