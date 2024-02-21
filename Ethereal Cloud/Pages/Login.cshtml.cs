using Ethereal_Cloud.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ethereal_Cloud.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string Password { get; set; }

        [BindProperty]
        public string Username { get; set; }

        public int errornum = -1;

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
                errornum = 0;
            }
        }
    }
}