using Ethereal_Cloud.Helpers;
using Ethereal_Cloud.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ethereal_Cloud.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public LoginDetails loginDetails { get; set; }

        public async Task OnPostLoginAsync()
        {
            if (!ModelState.IsValid)
            {
                Logger.LogToConsole(ViewData, "Invalid: Model error");
                return;
            }

            //create body object
            var dataObject = new Dictionary<string, object?>
            {
                { "Username", loginDetails.UsernameOrEmail }, //This is the username or email
                { "Password", loginDetails.Password }
            };

            //Make request
            var response = await ApiRequest.Files(ViewData, HttpContext, "v1/user/login", dataObject);

            Logger.LogToConsole(ViewData, "Checker: " + response);

            if (response != null)
            {
                //Valid login
                Logger.LogToConsole(ViewData, "Successfull login of user " + loginDetails.UsernameOrEmail);

                //Save authtoken as a cookie
                AuthTokenManagement.SetToken(HttpContext, response.ToString());

                //reset the folderpath cookie
                PathManagement.Remove(HttpContext);

                //goto the my files page
                Response.Redirect("/Upload");
            }
            else
            {
                Logger.LogToConsole(ViewData, "Invalid: Invalid Login");
                ViewData["FailureMessage"] = "Invalid login.";
            }
        }
    }
}