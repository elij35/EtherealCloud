namespace Ethereal_Cloud.Models
{
    public class FolderContentRecieve
    {
        public FileMetaRecieve[]? Files { get; set; }
        public FolderDataRecieve[]? Folders { get; set; }
    }

    public class FileMetaRecieve
    {
        public int FileID { get; set; }
        public string Filename { get; set; }
        public string Filetype { get; set; }
    }

    public class FolderDataRecieve
    {
        public int FolderID { get; set; }
        public string Foldername { get; set; }
    }
}