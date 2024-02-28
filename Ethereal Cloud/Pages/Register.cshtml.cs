using Ethereal_Cloud.Helpers;
using Ethereal_Cloud.Models.Signup;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ethereal_Cloud.Pages
{
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public SignupDetails signupDetails { get; set; }


        public async Task OnPostSignupAsync()
        {
            if (!ModelState.IsValid)
            {
                Logger.LogToConsole(ViewData, "Invalid: Model error");
                return;
            }

            //create body object
            var dataObject = new Dictionary<string, object?>
            {
                { "Username", signupDetails.Username },
                { "Email", signupDetails.Email },
                { "Password", signupDetails.Password }
            };

            //Make request
            var response = await ApiRequest.Files(ViewData, HttpContext, "v1/user/signup", dataObject);

            if (response != null)
            {
                //Valid Signup
                Logger.LogToConsole(ViewData, "Successfull signup of user " + signupDetails.Username);
                
                ViewData["SuccessMessage"] = "You have successfully registered.";
            }
            else
            {
                Logger.LogToConsole(ViewData, "Invalid: Couldn't signup");
                ViewData["FailureMessage"] = "Signup failed.";
            }
        }
    }
}