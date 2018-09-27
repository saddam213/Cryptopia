using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Cryptopia.DependencyInjection.MVC;
using Microsoft.AspNet.SignalR;
using System;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;

namespace Cryptopia.DependencyInjection
{
	public static class DependencyRegistrar
	{
		private static IWindsorContainer _container;
		private static string _assemblyName;

		public static void Register(string assembly)
		{
			_assemblyName = assembly;
			if (_container == null)
				_container = BootstrapContainer();

			var cryptopiaDependencyResolver = new CryptopiaDependencyResolver(_container.Kernel);
			DependencyResolver.SetResolver(cryptopiaDependencyResolver);
			GlobalHost.DependencyResolver = new SignalrDependencyResolver(_container.Kernel);
			GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerActivator), new WebApiDependencyResolver(_container));
		}

		public static void Deregister()
		{
			_container.Dispose();
		}

		public static void RegisterTransientComponent<T>(Func<T> factoryCreate) where T : class
		{
			_container
				.Register(Component.For<T>()
					.UsingFactoryMethod(factoryCreate)
					.LifestyleTransient());
		}

		public static IWindsorContainer Container
		{
			get { return _container; }
		}

		public static string AssemblyName
		{
			get { return _assemblyName; }
		}

		private static IWindsorContainer BootstrapContainer()
		{
			IWindsorContainer container = new WindsorContainer();
			container.Install(FromAssembly.This());
			return container;
		}
	}
}