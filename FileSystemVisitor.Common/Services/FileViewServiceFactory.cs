using FileSystemVisitor.Common.Interfaces;
using Microsoft.Extensions.Logging;
using System;

namespace FileSystemVisitor.Common.Services
{
    public class FileViewServiceFactory : IFileViewServiceFactory
    {
        private ILoggerFactory _loggerFactory;
        private ISystemEntityService _systemEntityService;

        public FileViewServiceFactory(ILoggerFactory loggerFactory, ISystemEntityService systemEntityService)
        {
            _loggerFactory = loggerFactory;
            _systemEntityService = systemEntityService;
        }

        public IFileViewService CreateNewService(Func<string[], string[]> filterAction)
        {
            return new FileViewService(_loggerFactory.CreateLogger<FileViewService>(), filterAction, _systemEntityService);
        }
    }
}
