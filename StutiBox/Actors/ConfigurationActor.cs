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
            var libraryConfigurationTest = File.ReadAllText("LibraryConfiguration.json");
            LibraryConfiguration = JsonConvert.DeserializeObject<dynamic>(libraryConfigurationTest);
        }
    }
}
