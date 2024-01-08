using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Ethereal_Cloud.Pages
{
    public class LoginModel : PageModel
    {

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public async Task OnPostLoginAsync()
        {
            string apiUrl = "http://82.47.161.57:8090/user/login";

            using (HttpClient client = new HttpClient())
            {
                var content = new StringContent($"{{\"username\":\"{Email}\",\"password\":\"{Password}\"}}", Encoding.UTF8, "application/json");
                var response = await client.PostAsync(apiUrl, content);


                if (response.IsSuccessStatusCode)
                {
                    string stringResponse = await response.Content.ReadAsStringAsync();

                    Response<string> login = await Response<string>.DeserializeJSON(stringResponse);

                    if (login.Success == true)
                    {
                        //Valid login goto the next page
                        ShowPopup(login.Message);

                        //Add redirect here!
                    }
                    else
                    {
                        ShowPopup("Invalid: " + login.Message);
                    }
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

    public class SignupModel : PageModel
    {
        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public async Task OnPostSignupAsync()
        {
            // Replace "your-api-endpoint" with the actual API endpoint
            string apiUrl = "http://82.47.161.57:8090/user/signup";

            using (HttpClient client = new HttpClient())
            {
                var content = new StringContent($"{{\"username\":\"{Username}\",\"email\":\"{Email}\",\"password\":\"{Password}\"}}", Encoding.UTF8, "application/json");
                var response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    string stringResponse = await response.Content.ReadAsStringAsync();

                    Response<string> signup = await Response<string>.DeserializeJSON(stringResponse);

                    if (signup.Success == true)
                    {
                        //Valid Signup goto the next page
                        ShowPopup(signup.Message);

                        //Add redirect link here!

                    }
                    else
                    {
                        ShowPopup("Invalid: " + signup.Message);
                    }
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