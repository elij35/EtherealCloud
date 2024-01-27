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
        public string Passwordconf { get; set; }

        public async Task OnPostLoginAsync()
        {
            //create body object
            var dataObject = new Dictionary<string, object?>
                {
                    { "Username", Username },
                    { "Email", Email },
                    { "Password", Password }
                };

            //Make request
            Message response = (Message)await ApiRequest.Files(ViewData, HttpContext, "v1/user/login", dataObject);

            if (response != null)
            {
                //Valid login
                Logger.LogToConsole(ViewData, "Successful: " + response.message);

                //save the auth token in a cookie
                var options = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, //for HTTPS
                };
                Response.Cookies.Append("AuthToken", response.message, options);

                //goto the my files page
                Response.Redirect("/Upload");
            }
            else
            {
                Logger.LogToConsole(ViewData, "Invalid: Couldn't signup");
            }
 
        }

        public async Task OnPostSignupAsync()
        {
            ViewData["logger"] = "Successful: test yay!";
            if (Passwordconf == Password)
            {
                //create body object
                var dataObject = new Dictionary<string, object?>
                    {
                        { "Username", Username },
                        { "Email", Email },
                        { "Password", Password }
                    };

                //Make request
                Message response = (Message)await ApiRequest.Files(ViewData, HttpContext, "v1/user/signup", dataObject);

                if(response != null)
                {
                    //Valid Signup
                    Logger.LogToConsole(ViewData, "Successful: " + response.message);
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