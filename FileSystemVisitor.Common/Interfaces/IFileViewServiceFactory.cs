using System;

namespace FileSystemVisitor.Common.Interfaces
{
    public interface IFileViewServiceFactory
    {
        IFileViewService CreateNewService(Func<string[], string[]> filterAction);
    }
}
