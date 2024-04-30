using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StorageController.Data.Models
{
    [Table("Folders", Schema = "ethereal")]
    public class Folder
    {

        [Key]
        [Required]
        public int FolderID { get; set; }

        [StringLength(30)]
        public string FolderName { get; set; }

        public int? ParentID { get; set; }

    }
}
