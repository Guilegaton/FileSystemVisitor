namespace FileSystemVisitor.Common.Models
{
    public delegate void StartEvent();
    public delegate void FinishEvent();
    public delegate void FileFindedEvent(ref FileViewerEventArgs args);
    public delegate void DirectoryFindedEvent(ref FileViewerEventArgs args);
    public delegate void FilteredFileFindedEvent(ref FileViewerEventArgs args);
    public delegate void FilteredDirectoryFindedEvent(ref FileViewerEventArgs args);
}
