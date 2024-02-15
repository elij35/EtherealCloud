using Ethereal_Cloud.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ethereal_Cloud.Pages
{
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string PasswordConf { get; set; }

        public int errornum = -1;

        public async Task OnPostSignupAsync()
        {
            if (PasswordConf == Password)
            {
                //create body object
                var dataObject = new Dictionary<string, object?>
                {
                    { "Username", Username },
                    { "Email", Email },
                    { "Password", Password }
                };

                //Make request
                var response = await ApiRequest.Files(ViewData, HttpContext, "v1/user/signup", dataObject);

                if (response != null)
                {
                    //Valid Signup
                    Logger.LogToConsole(ViewData, "Successfull signup of user " + Username);
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