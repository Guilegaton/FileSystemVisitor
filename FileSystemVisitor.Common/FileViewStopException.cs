using FileSystemVisitor.Common.Models;
using System;

namespace FileSystemVisitor.Common
{
    public class FileViewStopException : Exception
    {
        public DirectoryEntityCollection Collection { get; set; }
    }
}
