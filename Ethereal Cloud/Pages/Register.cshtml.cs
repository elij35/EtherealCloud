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


        public async Task<IActionResult> OnPostSignupAsync()
        {
            if (!ModelState.IsValid)
            {
                Logger.LogToConsole(ViewData, "Invalid: Model error");
                return Page();
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
                Logger.LogToConsole(ViewData, "Successful signup of user " + signupDetails.Username);

                ViewData["SuccessMessage"] = "You have successfully registered.";

                return RedirectToPage("/Login"); // Redirects to Login page after successfully registering
            }
            else
            {
                Logger.LogToConsole(ViewData, "Invalid: Couldn't signup");
                ViewData["FailureMessage"] = "Signup failed.";
                return Page();
            }
        }
    }
}