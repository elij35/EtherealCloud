using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Ethereal_Cloud.Models;
using System;

namespace Ethereal_Cloud.SignupController
{

    [ApiController]
    [Route("/signup")]
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
        [Consumes("application/json")]
        public async Task<IActionResult> SignupPostAsync([FromBody] SignupModel signup)
        {
            // Handle the posted data here, for example, save it to a database.
            var hi = await Post(signup);


            // Redirect to a different action or view
            return PartialView("_Popup", "Signup successful!");
        }

        private static async Task<string> Post(SignupModel signup)
        {
            // Create an object to represent the data
            var data = new
            {
                Username = signup.Username,
                Email = signup.Email,
                Password = signup.Password
            };

            // Convert the object to a JSON string
            string jsonData = JsonSerializer.Serialize(data);

            // Replace the URL with your actual API endpoint
            string apiUrl = "http://" + Environment.GetEnvironmentVariable("SC_IP") + ":8090/user/signup";

            // Send the POST request
            using (var httpClient = new HttpClient())
            {
                // Create a StringContent with JSON data
                var content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");

                // Send the POST request
                var response = await httpClient.PostAsync(apiUrl, content);

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("hui");
                    return "POST request successful!";
                }
                else
                {
                    return "POST request failed with status code: " + response.StatusCode;
                }
            }
        }


    }

}
