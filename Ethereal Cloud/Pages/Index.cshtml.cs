using Ethereal_Cloud.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;
using System.Web;

namespace Ethereal_Cloud.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public string Username { get; set; }

        public async Task OnPostFileAsync()
        {
            string apiUrl = "http://" + Environment.GetEnvironmentVariable("SC_IP") + ":8090/file";

            using (HttpClient client = new HttpClient())
            {
                var content = new StringContent($"{{\"username\":\"test\"}}", Encoding.UTF8, "application/json");
                var response = await client.PostAsync(apiUrl, content);
                string stringResponse = await response.Content.ReadAsStringAsync();

                Response<FileModel> file = await Response<FileModel>.DeserializeJSON(stringResponse);

                if (response.IsSuccessStatusCode)
                {
                    ShowPopup(file.Message.Content);
                }
                else
                {
                    ShowPopup("Failure");
                }
            }
        }

        private void ShowPopup(string status)
        {
            ViewData["PopupStatus"] = status;
        }
    }
}
