using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;

namespace Ethereal_Cloud.Pages
{
    public class LoginSignupModel : PageModel
    {
        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Passwordconf { get; set; }

        public async Task OnPostLoginAsync()
        {
            string apiUrl = "http://" + Environment.GetEnvironmentVariable("SC_IP") + ":8090/v1/user/login";

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
                        Response.Redirect("/Upload");
                    }
                    else
                    {
                        ShowPopup("Invalid: " + login.Message);
                    }
                }
            }
        }

        public async Task OnPostSignupAsync()
        {
            string apiUrl = "http://" + Environment.GetEnvironmentVariable("SC_IP") + ":8090/v1/user/signup";

            using (HttpClient client = new HttpClient())
            {
                var content = new StringContent($"{{\"username\":\"{Username}\",\"email\":\"{Email}\",\"password\":\"{Password}\",\"confpassword\":\"{Passwordconf}\"}}", Encoding.UTF8, "application/json");
                var response = await client.PostAsync(apiUrl, content);

                if (Passwordconf == Password)
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string stringResponse = await response.Content.ReadAsStringAsync();

                        Response<string> signup = await Response<string>.DeserializeJSON(stringResponse);

                        if (signup.Success == true)
                        {
                            //Valid Signup goto the next page
                            Response.Redirect("/Upload");
                        }
                        else
                        {
                            ShowPopup("Invalid: " + signup.Message);
                        }
                    }
                }
                else
                {
                    ShowPopup("Invalid: passwords must match!");
                }
            }
        }

        private void ShowPopup(string status)
        {
            ViewData["PopupStatus"] = status;
        }
    }
}