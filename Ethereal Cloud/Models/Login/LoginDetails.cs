using System.ComponentModel.DataAnnotations;

namespace Ethereal_Cloud.Models.Login
{
    public class LoginDetails
    {
        [Required(ErrorMessage = "Username or Email is required.")]
        [StringLength(64)]
        public string UsernameOrEmail { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(64)]
        public string Password { get; set; }
    }
}