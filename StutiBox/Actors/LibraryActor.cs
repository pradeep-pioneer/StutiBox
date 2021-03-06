﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using StutiBox.Models;
using Autofac;

namespace StutiBox.Actors
{
    public class LibraryActor : ILibraryActor
    {
		private IConfigurationActor configurationActor;
		private IBassActor bassActor;
		public DateTime RefreshedAt { get; private set; }
        public LibraryActor(IConfigurationActor configuration, IBassActor bass)
        {
			
			configurationActor = configuration??throw new ArgumentNullException(nameof(configuration));
			bassActor = bass ?? throw new ArgumentNullException(nameof(bass));
            buildLibrary(configuration, bass);
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

        public bool Refresh()
		{
			bool result;

			try
			{
				buildLibrary(configurationActor, bassActor);
				result = true;
			}
			catch
			{
				result = false;
			}
			return result;
		}

        private void buildLibrary(IConfigurationActor configuration, IBassActor bass)
        {
            LibraryItems = new List<LibraryItem>();
            int counter = 1;
            var directory = new DirectoryInfo(configuration.LibraryConfiguration.MusicDirectory.ToString());
			var comparison = new Comparison<FileInfo>((x, y) => { return x.Name.CompareTo(y.Name); });
			var musicFiles = directory.EnumerateFiles("*.mp3", SearchOption.AllDirectories).ToList();
			musicFiles.Sort(comparison);
			var items = musicFiles.Select(x => new LibraryItem(counter++, x.FullName, bassActor)).ToList();
			LibraryItems.AddRange(items);
			RefreshedAt = DateTime.Now;
        }
    }
}
