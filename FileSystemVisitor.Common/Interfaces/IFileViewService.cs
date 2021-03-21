using FileSystemVisitor.Common.Models;
using System.Collections.Generic;

namespace FileSystemVisitor.Common.Interfaces
{
    public interface IFileViewService
    {
        IEnumerable<string> ViewFiles(string path);
        event StartEvent Start;
        event StartEvent Finish;
        event FileFindedEvent FileFinded;
        event DirectoryFindedEvent DirectoryFinded;
        event FilteredFileFindedEvent FilteredFileFinded;
        event FilteredDirectoryFindedEvent FilteredDirectoryFinded;
    }
}
