using System;
namespace StutiBox.Models
{
    public class LibraryItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FullPath { get; set; }

		public override bool Equals(object obj)
		{
            return (obj as LibraryItem).FullPath == this.FullPath;
		}
	}
}
