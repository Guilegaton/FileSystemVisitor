using FileSystemVisitor.Common.Models;
using FileSystemVisitor.Common.Services;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Linq;

namespace FileSystemVisitor.Tests
{
    public class FileViewServiceTests
    {
        string[] _files;
        string[] _directories;
        string _path;
        Mock<SystemEntityService> _systemEntityService;
        Mock<ILogger<FileViewService>> _logger;

        [SetUp]
        public void Setup()
        {

            _files = new string[]
            {
                "file",
                "file1"
            };

            _directories = new string[]
            {
                            "directory",
                            "directory1"
            };

            _path = "test";

            _systemEntityService = new Mock<SystemEntityService>();
            _systemEntityService
                .Setup(service => service.GetFiles(It.IsAny<string>()))
                .Returns(_files);
            _systemEntityService
                .Setup(service => service.GetDirectories(_path))
                .Returns(_directories);
            _systemEntityService
                .Setup(service => service.GetDirectories(It.IsNotIn(new string[] { _path })))
                .Returns(new string[0]);
            _logger = new Mock<ILogger<FileViewService>>();
        }

        [Test]
        public void ViewFiles_NoEventsNoFilters_ReturnsCollectionOfFilesAndDirectoriesWithNumberInName()
        {
            // Arrange
            var service = new FileViewService(_logger.Object, null, _systemEntityService.Object);

            // Act
            var collection = service.ViewFiles(_path);

            //// Assert
            Assert.IsTrue(collection.Count(item => _files.Any(file => item.Contains(file))) == 6);
            Assert.IsTrue(collection.Count(item => _directories.Any(directory => item.Contains(directory))) == 2);
        }

        [Test]
        public void ViewFiles_NoEventsWithFilter_ReturnsCollectionOfFilesAndDirectories()
        {
            // Arrange
            var service = new FileViewService(_logger.Object, (arr) => arr.Where(item => item.Contains("1")).ToArray(), _systemEntityService.Object);

            // Act
            var collection = service.ViewFiles(_path);

            //// Assert
            Assert.IsTrue(collection.Count(item => _files.Any(file => item.Contains(file))) == 3);
            Assert.IsTrue(collection.Count(item => _directories.Any(directory => item.Contains(directory))) == 1);
        }

        [Test]
        public void ViewFiles_WithFileFindedEventNoFilter_ReturnsCollectionOfFilesWithoutDirectories()
        {
            // Arrange
            var service = new FileViewService(_logger.Object, null, _systemEntityService.Object);
            service.FileFinded += (ref FileViewerEventArgs args) =>
            {
                args.StopSearching = true;
            };

            // Act
            var collection = service.ViewFiles(_path);

            //// Assert
            Assert.IsTrue(collection.Count(item => _files.Any(file => item.Contains(file))) == 6);
            Assert.IsTrue(collection.Count(item => _directories.Any(directory => item.Contains(directory))) == 0);
        }

        [Test]
        public void ViewFiles_WithDirectoryFindedEventWithFilter_ReturnsCollectionDirectoriesWithoutFiles()
        {
            // Arrange
            var service = new FileViewService(_logger.Object, (arr) => arr.Where(item => item.Contains("1")).ToArray(), _systemEntityService.Object);
            service.DirectoryFinded += (ref FileViewerEventArgs args) =>
            {
                args.StopSearching = true;
            };

            // Act
            var collection = service.ViewFiles(_path);

            //// Assert
            Assert.IsTrue(collection.Count(item => _files.Any(file => item.Contains(file))) == 0);
            Assert.IsTrue(collection.Count(item => _directories.Any(directory => item.Contains(directory))) == 2);
        }

        [Test]
        public void ViewFiles_WithFilteredFileFindedEventWithFilter_ReturnsCollectionOfFilteredFilesAndNotFilteredDirectories()
        {
            // Arrange
            var service = new FileViewService(_logger.Object, (arr) => arr.Where(item => item.Contains("1")).ToArray(), _systemEntityService.Object);
            service.FilteredFileFinded += (ref FileViewerEventArgs args) =>
            {
                args.StopSearching = true;
            };

            // Act
            var collection = service.ViewFiles(_path);

            //// Assert
            Assert.IsTrue(collection.Count(item => _files.Any(file => item.Contains(file))) == 3);
            Assert.IsTrue(collection.Count(item => _directories.Any(directory => item.Contains(directory))) == 2);
        }

        [Test]
        public void ViewFiles_WithFilteredDirectoryFindedEventWithFilter_ReturnsCollectionOfFilteredDirectoriesAndNotFilteredFiles()
        {
            // Arrange
            var service = new FileViewService(_logger.Object, (arr) => arr.Where(item => item.Contains("1")).ToArray(), _systemEntityService.Object);
            service.FilteredDirectoryFinded += (ref FileViewerEventArgs args) =>
            {
                args.StopSearching = true;
            };

            // Act
            var collection = service.ViewFiles(_path);

            //// Assert
            Assert.IsTrue(collection.Count(item => _files.Any(file => item.Contains(file))) == 6);
            Assert.IsTrue(collection.Count(item => _directories.Any(directory => item.Contains(directory))) == 1);
        }

        [Test]
        public void ViewFiles_WithDirectoryFindedEventNoFilter_ReturnsCollectionOfDirectoriesWithoutRemovedDirectoriesAndWithoutFiles()
        {
            // Arrange
            var service = new FileViewService(_logger.Object, (arr) => arr.Where(item => item.Contains("1")).ToArray(), _systemEntityService.Object);
            service.DirectoryFinded += (ref FileViewerEventArgs args) =>
            {
                args.RemoveDirectories = new string[] { _directories[0] };
                args.StopSearching = true;
            };

            // Act
            var collection = service.ViewFiles(_path);

            //// Assert
            Assert.IsTrue(collection.Count(item => _files.Any(file => item.Contains(file))) == 0);
            Assert.IsTrue(collection.Count(item => _directories.Any(directory => item.Contains(directory))) == 1);
        }

        [Test]
        public void ViewFiles_WithFileFindedEventNoFilter_ReturnsCollectionOfFilesWithoutRemovedFilesAndWithoutDirectories()
        {
            // Arrange
            var service = new FileViewService(_logger.Object, (arr) => arr.Where(item => item.Contains("1")).ToArray(), _systemEntityService.Object);
            service.FileFinded += (ref FileViewerEventArgs args) =>
            {
                args.RemoveFiles = new string[] { _files[0] };
                args.StopSearching = true;
            };

            // Act
            var collection = service.ViewFiles(_path);

            //// Assert
            Assert.IsTrue(collection.Count(item => _files.Any(file => item.Contains(file))) == 3);
            Assert.IsTrue(collection.Count(item => _directories.Any(directory => item.Contains(directory))) == 0);
        }
    }
}
