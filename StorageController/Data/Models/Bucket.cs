using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StorageController.Data.Models
{
    [Table("Buckets", Schema = "ethereal")]
    public class Bucket
    {

        [Required]
        [Key]
        public int BucketID { get; set; }

        [Required]
        public string BucketIP { get; set; }

        [Required]
        public string BucketPort { get; set; }

        public ICollection<FileData> Files { get; set; }

    }
}
