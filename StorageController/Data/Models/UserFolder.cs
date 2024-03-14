using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StorageController.Data.Models
{
    [Table("UserFolders", Schema = "ethereal")]
    [PrimaryKey("FolderID", "UserID")]
    public class UserFolder
    {

        [Required]
        public int FolderID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        public string Privilege { get; set; }

        /// <summary>
        /// Foreign key section
        /// </summary>
        public Folder FolderData { get; set; }

        public User UserData { get; set; }

    }
}
