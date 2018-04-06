using System;
using System.IO;
using Newtonsoft.Json;

namespace StutiBox.Actors
{
    public class ConfigurationActor : IConfigurationActor
    {
        
        public dynamic LibraryConfiguration { get; protected set; }
        public ConfigurationActor()
        {
			string libraryConfiguration = string.Empty;
			if (Environment.OSVersion.Platform == PlatformID.Unix && (Environment.CurrentDirectory.Contains("pi")))
			{
				Console.WriteLine("This is a raspberry pi!");
				libraryConfiguration = File.ReadAllText("LibraryConfiguration.pi.json");
				Console.WriteLine($"Library Config:\n{libraryConfiguration}");
			}
			else if (Environment.OSVersion.Platform == PlatformID.Unix)
			{
				Console.WriteLine("This is a Mac!");
				libraryConfiguration = File.ReadAllText("LibraryConfiguration.mac.json");
				Console.WriteLine($"Library Config:\n{libraryConfiguration}");
			}
			if (!string.IsNullOrEmpty(libraryConfiguration))
				LibraryConfiguration = JsonConvert.DeserializeObject<dynamic>(libraryConfiguration);
			else
				throw new NullReferenceException($"The configuration doesn't exist for the platfrom {Environment.OSVersion.Platform.ToString()}");
        }
    }
}
