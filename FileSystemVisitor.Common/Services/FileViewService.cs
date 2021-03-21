using FileSystemVisitor.Common.Interfaces;
using FileSystemVisitor.Common.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileSystemVisitor.Common.Services
{
    public class FileViewService : IFileViewService
    {
        private ILogger _logger;
        private Func<string[], string[]> _filterAction;
        private ISystemEntityService _systemEntityService;
        private FileViewerEventArgs args;

        #region events

        public event StartEvent Start;
        public event StartEvent Finish;
        public event FileFindedEvent FileFinded;
        public event DirectoryFindedEvent DirectoryFinded;
        public event FilteredFileFindedEvent FilteredFileFinded;
        public event FilteredDirectoryFindedEvent FilteredDirectoryFinded;

        #endregion

        public FileViewService(ILogger<FileViewService> logger, Func<string[], string[]> filterAction, ISystemEntityService systemEntityService)
        {
            _logger = logger;
            _filterAction = filterAction;
            _systemEntityService = systemEntityService;
            args = new FileViewerEventArgs();
        }

        public IEnumerable<string> ViewFiles(string path)
        {
            _logger.LogInformation("Start file searching.");
            Start?.Invoke();

            var result = GetEntities(path);

            _logger.LogInformation("End file searching.");
            Finish?.Invoke();

            return result;
        }

        private DirectoryEntityCollection FilterDirectories(DirectoryEntityCollection collection)
        {
            collection.Directories = _filterAction?.Invoke(collection.Directories) ?? collection.Directories;

            for (var i =0; i < collection.SubEntities.Length; i++)
            {
                collection.SubEntities[i] = FilterDirectories(collection.SubEntities[i]);
            }

            FilteredDirectoryFinded?.Invoke(ref args);
            if (args.RemoveDirectories.Any())
            {
                collection.RemoveFolder(args.RemoveDirectories);
            }
            if (args.RemoveFiles.Any())
            {
                collection.RemoveFiles(args.RemoveFiles);
            }
            if (args.StopSearching)
            {
                return collection;
            }

            return collection;
        }

        private DirectoryEntityCollection FilterFiles(DirectoryEntityCollection collection)
        {
            collection.Files = _filterAction?.Invoke(collection.Files) ?? collection.Files;

            for (var i = 0; i < collection.SubEntities.Length; i++)
            {
                collection.SubEntities[i] = FilterFiles(collection.SubEntities[i]);
            }

            FilteredFileFinded?.Invoke(ref args);
            if (args.RemoveDirectories.Any())
            {
                collection.RemoveFolder(args.RemoveDirectories);
            }
            if (args.RemoveFiles.Any())
            {
                collection.RemoveFiles(args.RemoveFiles);
            }
            if (args.StopSearching)
            {
                return collection;
            }

            return collection;
        }

        private DirectoryEntityCollection GetEntities(string path)
        {
            var files = GetFiles(path);
            if (args.StopSearching)
            {
                return files;
            }
            args.Clear();

            var directories = GetDirectories(path);
            if (args.StopSearching)
            {
                return directories;
            }
            args.Clear();

            files.Join(directories);

            var filteredFiles = FilterFiles(files.Copy());
            if (args.StopSearching)
            {
                return filteredFiles;
            }
            args.Clear();

            var filteredDirectories = FilterDirectories(files.Copy());
            if (args.StopSearching)
            {
                return filteredDirectories;
            }
            args.Clear();

            var result = filteredFiles.Join(filteredDirectories);

            return result;
        }

        private DirectoryEntityCollection GetFiles(string path)
        {
            var result = new DirectoryEntityCollection();
            result.Files = _systemEntityService.GetFiles(path);

            var directories = _systemEntityService.GetDirectories(path);
            var subFolders = new List<DirectoryEntityCollection>();
            if (directories.Length > 0)
            {
                foreach (var item in directories)
                {
                    subFolders.Add(GetFiles(item));
                }
            }

            result.SubEntities = subFolders.ToArray();

            FileFinded?.Invoke(ref args);
            if (args.RemoveDirectories.Any())
            {
                result.RemoveFolder(args.RemoveDirectories);
            }
            if (args.RemoveFiles.Any())
            {
                result.RemoveFiles(args.RemoveFiles);
            }
            if (args.StopSearching)
            {
                return result;
            }

            return result;
        }

        private DirectoryEntityCollection GetDirectories(string path)
        {
            var result = new DirectoryEntityCollection();
            result.Directories = _systemEntityService.GetDirectories(path);

            var subFolders = new List<DirectoryEntityCollection>();
            if (result.Directories.Length > 0)
            {
                foreach (var item in result.Directories)
                {
                    subFolders.Add(GetDirectories(item));
                }
            }

            result.SubEntities = subFolders.ToArray();

            DirectoryFinded?.Invoke(ref args);
            if (args.RemoveDirectories.Any())
            {
                result.RemoveFolder(args.RemoveDirectories);
            }
            if (args.RemoveFiles.Any())
            {
                result.RemoveFiles(args.RemoveFiles);
            }
            if (args.StopSearching)
            {
                return result;
            }

            return result;
        }
    }
}
