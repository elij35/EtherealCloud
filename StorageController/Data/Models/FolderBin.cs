using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StorageController.Data.Models
{
    [Table("FolderBin", Schema = "ethereal")]
    [PrimaryKey("FolderID")]
    public class FolderBin
    {

        [Required]
        public int FolderID { get; set; }

        /// <summary>
        /// Foreign keys
        /// </summary>
        public Folder FolderData { get; set; }

    }
}
