namespace FileSystemVisitor.Common.Interfaces
{
    public interface ISystemEntityService
    {
        string[] GetFiles(string path);
        string[] GetDirectories(string path);
    }
}
