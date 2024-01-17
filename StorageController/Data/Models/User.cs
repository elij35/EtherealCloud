using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StorageController.Data.Models
{

    [Table("Users", Schema = "ethereal")]
    public class User
    {

        [Key]
        public int UserID { get; set; }

        [Required]
        [StringLength(64)]
        public string Username { get; set; }

        [Required]
        [StringLength(64)]
        public string Email { get; set; }

        [Required]
        [StringLength(64)]
        public string Password { get; set; }

        [Required]
        [StringLength(20)]
        public string PasswordSalt { get; set; }

        [Required]
        public bool Administrator { get; set; }

        public ICollection<UserFile> UserFiles { get; set; }

    }

}
