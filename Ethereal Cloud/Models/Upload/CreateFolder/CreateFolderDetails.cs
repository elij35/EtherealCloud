using System.ComponentModel.DataAnnotations;

namespace Ethereal_Cloud.Models.Upload.CreateFolder
{
    public class CreateFolderDetails
    {
        [Required(ErrorMessage = "Folder name is required.")]
        [StringLength(30)]
        public string FolderName { get; set; }
    }
}