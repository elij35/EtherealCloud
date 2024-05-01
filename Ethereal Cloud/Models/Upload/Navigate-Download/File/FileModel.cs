using System.ComponentModel.DataAnnotations;

namespace Ethereal_Cloud.Models.Upload.Get.File
{
    public class FileModel
    {
        [Required]
        [StringLength(64)]
        public string Filename { get; set; }

        [Required]
        [StringLength(255)]
        public string Filetype { get; set; }
        public string Content { get; set; }
    }
}