using System.ComponentModel.DataAnnotations;

namespace Ethereal_Cloud.Models
{
    public class DownNavDetails
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}