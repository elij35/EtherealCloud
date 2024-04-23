using System.ComponentModel.DataAnnotations;

namespace Ethereal_Cloud.Models.Upload.Share
{
    public class ShareDetails
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public int Id { get; set; }
    }
}
