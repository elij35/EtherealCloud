using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StorageController.Data.Models
{
    [Table("Files", Schema = "ethereal")]
    public class FileData
    {

        [Key]
        public int FileID { get; set; }

        [StringLength(64)]
        public string FileType { get; set; }

        [Required]
        [StringLength(64)]
        public string FileName { get; set; }

        [StringLength(64)]
        public string? FilePassword { get; set; }

        public DateTime? CreationDate { get; set; }

        public int? RedundantID { get; set; }

        [Required]
        public int BucketID { get; set; }

        public int? FolderID { get; set; }

        /// <summary>
        /// Foreign key section
        /// </summary>
        public ICollection<UserFile> UserFiles { get; set; }
        public Folder? FolderData { get; set; }
        public Bucket BucketLocation { get; set; }

    }
}
