using Ethereal_Cloud.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;
using System.Text;

namespace Ethereal_Cloud.Pages
{
    public class LoginSignupModel : PageModel
    {
        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string PasswordConf { get; set; }

        public async Task OnPostLoginAsync()
        {
            //create body object
            var dataObject = new Dictionary<string, object?>
                {
                    { "Username", Username }, //This is the username or email
                    { "Password", Password }
                };

            //Make request
            var response = await ApiRequest.Files(ViewData, HttpContext, "v1/user/login", dataObject);

            Logger.LogToConsole(ViewData, "Checker: " + response);
            
            if (response != null)
            {
                //Valid login
                Logger.LogToConsole(ViewData, "Successfull login of user " + Username);

                //save the auth token in a cookie
                var options = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, //for HTTPS
                };
                //response
                Response.Cookies.Append("AuthToken", response.ToString(), options);

                //goto the my files page
                Response.Redirect("/Upload");
            }
            else
            {
                Logger.LogToConsole(ViewData, "Invalid: Invalid Login");
            }
        }

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

                if(response != null)
                {
                    //Valid Signup
                    Logger.LogToConsole(ViewData, "Successfull signup of user " + Username);
                }
                else
                {
                    Logger.LogToConsole(ViewData, "Invalid: Couldn't signup");
                }
            }
            else
            {
                Logger.LogToConsole(ViewData, "Invalid: passwords must match!");
            }
            
        }
    }
}