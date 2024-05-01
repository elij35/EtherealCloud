using System.ComponentModel.DataAnnotations;

namespace Ethereal_Cloud.Models.Upload.Rename
{
    public class RenameDetails
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
