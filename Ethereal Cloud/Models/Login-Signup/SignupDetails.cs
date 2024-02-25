using Microsoft.AspNetCore.Mvc;

namespace Ethereal_Cloud.Models
{
    public class SignupDetails
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordConf { get; set; }
    }
}