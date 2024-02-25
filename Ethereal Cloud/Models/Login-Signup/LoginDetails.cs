using System.ComponentModel.DataAnnotations;

namespace Ethereal_Cloud.Models
{
    public class LoginDetails
    {
        [Required(ErrorMessage = "Username or Email is required.")]
        public string UsernameOrEmail { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
    }
}