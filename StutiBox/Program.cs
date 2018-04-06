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
		/*
		 * Add healthcheck endpoint for service healthchecking
        */
        public static void Main(string[] args)
        {
			Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
			Console.WriteLine($"StutiBox service starting up in '{Environment.CurrentDirectory}/'...");
			Console.WriteLine($"Detected Platform: {Environment.OSVersion.Platform.ToString()}");
			Console.WriteLine($"Version: {Environment.OSVersion.VersionString}");
			Console.WriteLine($"Host: {Environment.MachineName}");
			Console.WriteLine($"Processors: {Environment.ProcessorCount}");
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
