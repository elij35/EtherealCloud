using Ethereal_Cloud.Helpers;
using Ethereal_Cloud.Models.Login;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StorageController.Data.Models;
using System.Text.Json;

namespace Ethereal_Cloud.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public LoginDetails loginDetails { get; set; }

        public async Task OnGet()
        {
            //Clear all of the previous cookies
            HttpContext.Session.Clear();
        }


        public async Task OnPostLoginAsync()
        {
            if (!ModelState.IsValid)
            {
                return;
            }

            //create body object
            var dataObject = new Dictionary<string, object?>
            {
                { "Username", loginDetails.UsernameOrEmail }, //This is the username or email
                { "Password", loginDetails.Password }
            };

            //Make request
            var response = await ApiRequest.Files(HttpContext, "v1/user/login", dataObject);

            if (response != null)
            {
                //Valid login

                //Put response in form of FolderContentRecieve
                string jsonString = response.ToString();
                AuthDetails authDetails = JsonSerializer.Deserialize<AuthDetails>(jsonString);


                //Save authtoken as a cookie
                CookieManagement.SetCookie(HttpContext, "AuthToken", authDetails.Token);

                //Save email
                CookieManagement.SetCookie(HttpContext, "Email", authDetails.Email);

                //goto the my files page
                Response.Redirect("/Auth");
            }
            else
            {
                ViewData["FailureMessage"] = "Invalid login.";
            }
        }
    }
}