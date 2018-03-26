using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StutiBox.Actors;

namespace StutiBox
{
    public class Program
    {
        public static void Main(string[] args)
        {
            SetupDI();
            var config = DependencyActor.Container.Resolve<IConfigurationActor>();
            Console.WriteLine($"{config.LibraryConfiguration.MusicDirectory}");
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                   .UseUrls("http://*:5000")
                   .Build();

        public static void SetupDI() => DependencyActor.RegisterDependencies();
    }
}
