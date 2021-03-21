using FileSystemVisitor.Common.Interfaces;
using System.IO;

namespace FileSystemVisitor.Common.Services
{
    public class SystemEntityService : ISystemEntityService
    {
        public virtual string[] GetFiles(string path)
        {
            return Directory.GetFiles(path);
        }

        public virtual string[] GetDirectories(string path)
        {
            return Directory.GetDirectories(path);
        }
    }
}
