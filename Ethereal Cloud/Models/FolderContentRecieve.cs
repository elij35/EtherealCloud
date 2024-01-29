namespace Ethereal_Cloud.Pages
{
    public class FolderContentRecieve
    {
        public FileMetaRecieve[]? Files { get; set; }
        public FolderDataRecieve[]? Folders { get; set; }
    }

    public class FileMetaRecieve
    {
        public int FileId { get; set; }
        public string Filename { get; set; }
        public string Filetype { get; set; }
    }

    public class FolderDataRecieve
    {
        public int FolderId { get; set; }
        public string FolderName { get; set; }
    }
}