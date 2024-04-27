using System.ComponentModel.DataAnnotations;

namespace Ethereal_Cloud.Models.Upload.Get.Folder
{
    public class ShareContentDisplay
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }

        public List<SharingUsers> SharingUsers { get; set; }
    }

    public class SharingUsers
    {
        public string Username { get; set; }

        public int UserID { get; set; }
    }


    public class ShareFileMetaRecieve
    {
        public int FileID { get; set; }

        [Required]
        [StringLength(64)]
        public string Filename { get; set; }

        [Required]
        [StringLength(64)]
        public string Filetype { get; set; }

        public List<SharingUsers> SharingUsers { get; set; }
    }


}