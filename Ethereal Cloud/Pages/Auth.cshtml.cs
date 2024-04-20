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
        public void OnGet()
        {
            // Handle GET request
            
        }

        public IActionResult OnPost(string digit1, string digit2, string digit3, string digit4, string digit5, string digit6)
        {
            string authCode = digit1 + digit2 + digit3 + digit4 + digit5 + digit6;

            // Replace this with your actual authentication logic
            string email = "EtherealCloudTesting@outlook.com";
            string pass = "=pew9V_s";
            string userEmail = "rileycoulstock@gmail.com";
            
            SmtpClient client = new SmtpClient("smtp-mail.outlook.com");
            client.Port = 587;
            client.EnableSsl = true;

            client.Credentials = new NetworkCredential(email, pass);

            MailMessage message = new MailMessage(email, userEmail);
            message.Subject = "Test";
            message.Body = "Put the code here";
            try
            {
                client.Send(message);

            }
            catch (Exception)
            {
                throw;
            }
            Random random = new Random();
            int genCode = random.Next(100000, 1000000);
            string expectedCode = $"{genCode}";

            if (authCode == "123456")
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