using Ethereal_Cloud.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NuGet.Configuration;
using System;
using System.Net;
using System.Net.Mail;

namespace Ethereal_Cloud.Pages
{
    public class AuthModel : PageModel
    {
        private string? code = null;

        public void OnGet()
        {
            code = null;

            //Sends the user back to login if no auth token present
            CookieManagement.GetAuthToken(HttpContext);

            code = EmailManagement.Send2FAEmail();
        }

        public IActionResult OnPost(string digit1, string digit2, string digit3, string digit4, string digit5, string digit6)
        {
            string authCode = digit1 + digit2 + digit3 + digit4 + digit5 + digit6;

            // Replace this with your actual authentication logic
            

            if (authCode == code)
            {
                // Authentication successful, redirect to the home page or any other page
                return RedirectToPage("/Upload");
            }
            else
            {
                // Authentication code is incorrect
                ViewData["ErrorMessage"] = "Incorrect authentication code.";
            }

            // If authentication fails, return the page with error message
            return Page();
        }
    }
}