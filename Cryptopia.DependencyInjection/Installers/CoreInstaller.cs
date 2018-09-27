using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Cryptopia.DependencyInjection.Installers
{
	public class CoreInstaller : IWindsorInstaller
	{
		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			container.Register(Classes.FromAssemblyContaining<Cryptopia.Core.Core>()
				.Pick()
				.WithService
				.DefaultInterfaces()
				.LifestyleTransient()
				);

			container.Register(Classes.FromAssemblyContaining<Cryptopia.Data.Data>()
				.Pick()
				.WithService
				.DefaultInterfaces()
				.LifestyleTransient()
				);

			container.Register(Classes.FromAssemblyContaining<Cryptopia.Common.Common>()
				.Pick()
				.WithService
				.DefaultInterfaces()
				.LifestyleTransient()
				);

			container.Register(Classes.FromAssemblyContaining<Cryptopia.Admin.Core.AdminCore>()
				.Pick()
				.WithService
				.DefaultInterfaces()
				.LifestyleTransient()
				);

			container.Register(Classes.FromAssemblyContaining<Cryptopia.Infrastructure.Email.EmailService>()
				.Pick()
				.WithService.DefaultInterfaces()
				.LifestyleTransient()
				);
		}
	}
}