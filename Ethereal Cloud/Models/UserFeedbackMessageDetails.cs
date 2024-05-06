using System.ComponentModel.DataAnnotations;

namespace Ethereal_Cloud.Models
{
    public class UserFeedbackMessage
    {
        public bool ResultSuccess { get; set; }

        public string Message { get; set; }
    }
}