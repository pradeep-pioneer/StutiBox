using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using StutiBox.Models;
using Autofac;

namespace StutiBox.Actors
{
    public class LibraryActor : ILibraryActor
    {
        public LibraryActor(IConfigurationActor configuration)
        {
            buildLibrary(configuration);
        }

        public LibraryItem this[int id]=> GetItem(id);

        public LibraryItem GetItem(int id) => LibraryItems.FirstOrDefault(x => x.Id == id);

        public List<LibraryItem> LibraryItems { get; private set; }

        public List<LibraryItem> Find(params string[] keywords)
        {
            List<LibraryItem> results = new List<LibraryItem>();
            keywords.ToList().ForEach(item => { 
                var items = LibraryItems.Where(x => x.Name.ToLower().Contains(item.ToLower()));
                results.AddRange(items);
            });
            return results.Distinct().ToList();
        }

        public LibraryItem LuckySearch(params string[] keywords)
        {
            return LibraryItems.FirstOrDefault(item => keywords.Any(that => item.Name.ToLower().Contains(that.ToLower())));
        }

        public bool Refresh(bool stopPlayer = false)
		{
			bool result;
			if (stopPlayer)
				result = DependencyActor.Container.Resolve<IPlayerActor>().Stop();
			try
			{
				buildLibrary(DependencyActor.Container.Resolve<IConfigurationActor>());
				result = true;
			}
			catch
			{
				result = false;
			}
			return result;
		}

        private void buildLibrary(IConfigurationActor configuration)
        {
            LibraryItems = new List<LibraryItem>();
            int counter = 1;
            var directory = new DirectoryInfo(configuration.LibraryConfiguration.MusicDirectory.ToString());
            var musicFiles = directory.EnumerateFiles("*.mp3", SearchOption.AllDirectories);
            musicFiles.ToList().ForEach(fileInfo =>
            {
                LibraryItems.Add(new LibraryItem() { Id = counter++, Name = fileInfo.Name, FullPath = fileInfo.FullName });
            });
        }
    }
}
