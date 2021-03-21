using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FileSystemVisitor.Common.Models
{
    public class DirectoryEntityCollection : IEnumerable<string>
    {
        public string[] Files { get; internal set; }
        public string[] Directories { get; internal set; }
        public DirectoryEntityCollection[] SubEntities { get; internal set; }

        public ulong Length => (ulong)(Files.Length + Directories.Length + SubEntities.Sum(subEntity => (decimal)subEntity.Length));

        public DirectoryEntityCollection(string[] files, string[] directories)
        {
            Files = files;
            Directories = directories;
            SubEntities = new DirectoryEntityCollection[0];
        }

        public DirectoryEntityCollection() 
        {
            Files = new string[0];
            Directories = new string[0];
            SubEntities = new DirectoryEntityCollection[0];
        }

        public IEnumerator<string> GetEnumerator()
        {
            for (int i = 0; i < Directories.Length; i++)
            {
                yield return $"Fodler: {Directories[i]}";
            }
            for (int i = 0; i < Files.Length; i++)
            {
                yield return $"File: {Files[i]}";
            }
            for (int i = 0; i < SubEntities.Length; i++)
            {
                foreach (var item in SubEntities[i] ?? Enumerable.Empty<string>())
                {
                    yield return item;
                }
            }
        }

        protected internal void RemoveFolder(string[] directoryNames)
        {
            Directories = Directories.Where(directory => !directoryNames.Any(remDir => directory == remDir)).ToArray();
            foreach (var subEntity in SubEntities)
            {
                subEntity.RemoveFolder(directoryNames);
            }
        }

        protected internal void RemoveFiles(string[] fileNames)
        {
            Files = Files.Where(file => !fileNames.Any(remFile => file == remFile)).ToArray();
            foreach (var subEntity in SubEntities)
            {
                subEntity.RemoveFolder(fileNames);
            }
        }

        protected internal DirectoryEntityCollection Join(DirectoryEntityCollection collection)
        {
            Directories = collection.Directories;
            foreach (var fileCollection in SubEntities)
            {
                foreach (var directoryCollection in collection.SubEntities)
                {
                    fileCollection.Join(directoryCollection);
                }
            }

            return this;
        }

        protected internal DirectoryEntityCollection Copy()
        {
            var result = new DirectoryEntityCollection() {
                Directories = new string[this.Directories.Length],
                Files = new string[this.Files.Length],
                SubEntities = new DirectoryEntityCollection[this.SubEntities.Length]
            };
            Directories.CopyTo(result.Directories, 0);
            Files.CopyTo(result.Files, 0);

            for (int i = 0; i < this.SubEntities.Length; i++)
            {
                result.SubEntities[i] = SubEntities[i].Copy();
            }

            return result;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
