namespace FileSystemVisitor.Common.Models
{
    public class FileViewerEventArgs
    {
        public bool StopSearching { get; set; }
        public string[] RemoveDirectories { get; set; }
        public string[] RemoveFiles { get; set; }

        public FileViewerEventArgs()
        {
            RemoveDirectories = new string[0];
            RemoveFiles = new string[0];
        }

        public void Clear()
        {
            StopSearching = false;
            RemoveDirectories = new string[0];
            RemoveFiles = new string[0];
        }
    }
}
