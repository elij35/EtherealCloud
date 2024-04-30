using Ethereal_Cloud.Helpers;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ethereal_Cloud.Pages
{
    public class AuthModel : PageModel
    {
        public void OnGet()
        {
            string? code;

            string? email = CookieManagement.Get(HttpContext, "Email");

            //Email failed
            if (email == null)
            {
                RedirectToPage("/Login");
                return;
            }

            code = EmailManagement.Send2FAEmail(email);

            //code failed
            if (code == null)
            {
                RedirectToPage("/Login");
                return;
            }

            CookieManagement.SetCookie(HttpContext, "Code", code);
        }

        public async Task OnPost(string digit1, string digit2, string digit3, string digit4, string digit5, string digit6)
        {
            string authCode = digit1 + digit2 + digit3 + digit4 + digit5 + digit6;
            string? code = HttpContext.Session.GetString("Code");

            if (authCode == code)
            {
                // Authentication successful, redirect to the home page or any other page
                Response.Redirect("/Upload");
            }
            else
            {
                // Authentication code is incorrect
                ViewData["ErrorMessage"] = "Incorrect authentication code.";

                Response.Redirect("/Login");
            }
        }
    }
}