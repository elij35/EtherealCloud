using System.ComponentModel.DataAnnotations;

namespace Ethereal_Cloud.Models.Account
{
    public class ChangePasswordDetails
    {
        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
    }
}