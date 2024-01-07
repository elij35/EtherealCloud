using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;

namespace Ethereal_Cloud.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        [BindProperty]
        public string Username { get; set; }

        //Login:
        public async Task OnPostLoginAsync()
        {
            string apiUrl = "http://http://82.47.161.57/:8090/user/login";

            using (HttpClient client = new HttpClient())
            {
                var content = new StringContent($"{{\"username\":\"{Email}\",\"password\":\"{Password}\"}}", Encoding.UTF8, "application/json");
                var response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    ShowPopup("Success");
                }
                else
                {
                    ShowPopup("Failure");
                }
            }
        }

        //Register:
        public async Task OnPostSignupAsync()
        {
            string apiUrl = "http://http://82.47.161.57/:8090/user/signup";

            using (HttpClient client = new HttpClient())
            {
                var content = new StringContent($"{{\"username\":\"{Username}\",\"email\":\"{Email}\",\"password\":\"{Password}\"}}", Encoding.UTF8, "application/json");
                var response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    ShowPopup("Success");
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