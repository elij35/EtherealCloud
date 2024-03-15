using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StorageController.Data.Models
{
    [Table("FileBin", Schema = "ethereal")]
    [PrimaryKey("DeleteID")]
    public class FileBin
    {

        [Required]
        public int DeleteID { get; set; }

        [Required]
        public int FileID { get; set; }

        /// <summary>
        /// Foreign keys
        /// </summary>
        public FileData File { get; set; }

    }
}
