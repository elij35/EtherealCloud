using System.ComponentModel.DataAnnotations;

namespace Ethereal_Cloud.Models.Upload.Get
{
    public class FolderContentRecieve
    {
        public FileMetaRecieve[]? Files { get; set; }
        public FolderDataRecieve[]? Folders { get; set; }
    }
    
    public class FileMetaRecieve
    {
        public int FileID { get; set; }

        [Required]
        [StringLength(64)]
        public string Filename { get; set; }

        [Required]
        [StringLength(64)]
        public string Filetype { get; set; }
    }

    public class FolderDataRecieve
    {
        public int FolderID { get; set; }

        [StringLength(30)]
        public string Foldername { get; set; }
    }

}