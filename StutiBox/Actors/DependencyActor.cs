using System;
using Autofac;

namespace StutiBox.Actors
{
    public static class DependencyActor
    {
        public static IContainer Container { get; private set; }
        public static void RegisterDependencies()
        {
            var builder = new ContainerBuilder();
			builder.RegisterType<BassActor>().As<IBassActor>().SingleInstance();
            builder.RegisterType<ConfigurationActor>().As<IConfigurationActor>().SingleInstance();
            builder.RegisterType<LibraryActor>().As<ILibraryActor>().SingleInstance();
			builder.RegisterType<PlayerActor>().As<IPlayerActor>().SingleInstance();
            Container = builder.Build();
        }
    }
}
