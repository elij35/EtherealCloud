using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StorageController.Data.Models
{
    [Table("UserFiles", Schema = "ethereal")]
    [PrimaryKey("FileID", "UserID")]
    public class UserFile
    {

        [Required]
        public int FileID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        [StringLength(10)]
        public string Privilege { get; set; }

        public User UserData { get; set; }
        public FileData File { get; set; }

    }
}
