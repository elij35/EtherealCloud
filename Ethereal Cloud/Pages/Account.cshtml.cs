using Ethereal_Cloud.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ethereal_Cloud.Pages
{
    public class AccountModel : PageModel
    {


        public static async Task<string> Main()
        {
            return await SendGetRequest();
        }

        static async Task<string> SendGetRequest()
        {
            using (HttpClient client = new HttpClient())
            {
                string url = "http://" + Environment.GetEnvironmentVariable("SC_IP") + ":8090/api/auth";

                HttpResponseMessage response = await client.PostAsync(url, new StringContent("email=test@test.com&password=password123"));

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    return content;
                }
                else
                {
                    return response.StatusCode.ToString();
                }
            }
        }


        public IActionResult OnPost()
        {

            return RedirectToPage();
        }


    }

}
