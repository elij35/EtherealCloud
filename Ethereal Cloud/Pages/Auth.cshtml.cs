using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ethereal_Cloud.Pages
{
    public class AuthModel : PageModel
    {
        public void OnGet()
        {
            // Handle GET request
        }

        public IActionResult OnPost(string digit1, string digit2, string digit3, string digit4, string digit5, string digit6)
        {
            string authCode = digit1 + digit2 + digit3 + digit4 + digit5 + digit6;

            // Replace this with your actual authentication logic
            string expectedCode = "123456";

            if (authCode == expectedCode)
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