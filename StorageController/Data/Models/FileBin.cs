using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StorageController.Data.Models
{
    [Table("FileBin", Schema = "ethereal")]
    [PrimaryKey("FileID")]
    public class FileBin
    {

        [Required]
        public int FileID { get; set; }

        /// <summary>
        /// Foreign keys
        /// </summary>
        public FileData FileData { get; set; }

    }
}
