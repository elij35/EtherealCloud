using System.ComponentModel.DataAnnotations;

namespace Ethereal_Cloud.Models.Upload.CreateFolder
{
    public class CreateFolderDetails
    {
        [Required(ErrorMessage = "Folder name is required.")]
        public string FolderName { get; set; }
    }
}