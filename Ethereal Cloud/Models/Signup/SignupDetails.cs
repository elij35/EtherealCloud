using System.ComponentModel.DataAnnotations;

namespace Ethereal_Cloud.Models.Signup
{
    // lengths
    public class SignupDetails
    {
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(64)]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Not a Email.")]
        [StringLength(64)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [StringLength(64)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!#$%^&()_+\-=[\]{};':""\\|,.<>\/?]).{10,}$", ErrorMessage = "Password must contain at least 10 characters with 1 lowercase, 1 uppercase, 1 special and 1 number")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [StringLength(64)]
        public string PasswordConf { get; set; }
    }
}