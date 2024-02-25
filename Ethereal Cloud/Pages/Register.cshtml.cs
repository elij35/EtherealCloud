using Ethereal_Cloud.Helpers;
using Ethereal_Cloud.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ethereal_Cloud.Pages
{
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public SignupDetails signupDetails { get; set; }

        public int errornum = -1;

        public async Task OnPostSignupAsync()
        {
            if (signupDetails.PasswordConf == signupDetails.Password)
            {
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
                    errornum = 0;
                }
                else
                {
                    Logger.LogToConsole(ViewData, "Invalid: Couldn't signup");
                    errornum = 1;
                }
            }
            else
            {
                Logger.LogToConsole(ViewData, "Invalid: passwords must match!");
                errornum = 2;
            }
        }
    }
}