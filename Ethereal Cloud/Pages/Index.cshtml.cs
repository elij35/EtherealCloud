using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;
using System.Web;
using System.Net.Http.Headers;

namespace Ethereal_Cloud.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public string Username { get; set; }

        // Helper method to get or initialize the Files list from session
        public List<FileModel> Files
        {
            get
            {
                if (HttpContext.Session.TryGetValue("Files", out byte[] data) && data != null)
                {
                    string json = Encoding.UTF8.GetString(data);
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<List<FileModel>>(json);
                }
                else
                {
                    return new List<FileModel>();
                }
            }
            set
            {
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(value);
                HttpContext.Session.Set("Files", Encoding.UTF8.GetBytes(json));
            }
        }

        public async Task OnPostFileAsync()
        {
            string apiUrl = "http://" + Environment.GetEnvironmentVariable("SC_IP") + ":8090/file";

            using (HttpClient client = new HttpClient())
            {
                var content = new StringContent($"{{\"username\":\"{Username}\"}}", Encoding.UTF8, "application/json");
                var response = await client.PostAsync(apiUrl, content);

                string stringResponse = await response.Content.ReadAsStringAsync();

                Response<FileModel> file = await Response<FileModel>.DeserializeJSON(stringResponse);

                
                if (response.IsSuccessStatusCode)
                {
                    var files = Files;
                    files.Add(file.Message);
                    Files = files;
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

        

        public IActionResult OnGetDownload(string filename)
        {
            var files = Files;

            var file = files?.FirstOrDefault(f => f.Filename == filename);


            if (file == null)
            {
                return NotFound();
            }

            byte[] fileContents = Encoding.UTF8.GetBytes(file.Content);

            return File(fileContents, "text/plain", file.Filename + "." + file.Filetype);
        }


        public async Task<IActionResult> OnPostUploadAsync(IFormFile uploadedFile)
        {
            if (uploadedFile != null && uploadedFile.Length > 0)
            {
                using (var stream = new MemoryStream())
                {
                    await uploadedFile.CopyToAsync(stream);
                    var newFile = new FileModel
                    {
                        Filename = uploadedFile.FileName,
                        Filetype = Path.GetExtension(uploadedFile.FileName),
                        Content = Convert.ToBase64String(stream.ToArray())
                    };

                    var files = Files;
                    files.Add(newFile);
                    Files = files;

                    ShowPopup("File uploaded successfully");
                }
            }
            else
            {
                ShowPopup("Invalid file upload");
            }

            return RedirectToPage("Index");
        }








    }
}
