using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bucket.Data
{
    [Table("FileData")]
    public class FileData
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int FileID { get; set; }
        public string FilePath { get; set; }

    }
}
