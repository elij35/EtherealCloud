using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Ethereal_Cloud.Models;

namespace Ethereal_Cloud.SignupController
{
    public class SignupController : Controller
    {
        public IActionResult Signup()
        {
            return View();
        }

        public IActionResult Index()
        {
            // Your logic for the Index action goes here
            return View();
        }

        // POST: /YourController/HandlePost
        [HttpPost]
        public async Task<IActionResult> SignupPostAsync(SignupModel signup)
        {
            // Handle the posted data here, for example, save it to a database.

            var hi = await Post(signup);

            // Redirect to a different action or view
            return PartialView("_Popup", "Signup successful!");
        }

        private static async Task<string> Post(SignupModel signup)
        {
            // Replace these values with your actual data
            string username = signup.Username;
            string email = signup.Email;
            string password = signup.Password;

            // Create an object to represent the data
            var data = new
            {
                Username = username,
                Email = email,
                Password = password
            };

            // Convert the object to a JSON string
            string jsonData = JsonSerializer.Serialize(data);

            // Replace the URL with your actual API endpoint
            string apiUrl = "http://" + Environment.GetEnvironmentVariable("SC_IP") + ":8090/user/login";

            // Send the POST request
            var hi = await SendPostRequestAsync(apiUrl, jsonData);
            return hi;
        }

        private static async Task<string> SendPostRequestAsync(string apiUrl, string jsonData)
        {
            using (var httpClient = new HttpClient())
            {
                // Create a StringContent with JSON data
                var content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");

                // Send the POST request
                var response = await httpClient.PostAsync(apiUrl, content);

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    return "POST request successful!";
                }
                else
                {
                    return "POST request failed with status code: " +  response.StatusCode;
                }
            }
        }

    }

}
