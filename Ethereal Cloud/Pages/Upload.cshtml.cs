using Ethereal_Cloud.Pages;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Web;
using System.Net.Http.Headers;
using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.StaticFiles;

namespace Ethereal_Cloud.Pages
{
    public class UploadModel : PageModel
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
            bool fileFound = true;
            int counter = 1;
            while (fileFound)
            {
                string apiUrl = "http://" + Environment.GetEnvironmentVariable("SC_IP") + ":8090/file/" + counter;

                using (HttpClient client = new HttpClient())
                {
                    var content = new StringContent($"{{\"authtoken\":\"Temp\"}}", Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        string stringResponse = await response.Content.ReadAsStringAsync();
                        Response<object> responseObject = await Response<object>.DeserializeJSON(stringResponse);

                        if (responseObject.Success)
                        {
                            Response<FileModel> file = await Response<FileModel>.DeserializeJSON(stringResponse);
                            ShowPopup(file.Message.Content);
                            var files = Files;
                            files.Add(file.Message);
                            Files = files;
                        }
                        else
                        {
                            ShowPopup("Response failed: " + responseObject.Message);
                            fileFound = false;
                        }

                    }
                    else
                    {
                        ShowPopup("Failure");
                    }

                }


                counter++;
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

            byte[] fileContents = Convert.FromBase64String(file.Content);

            // Determine content type based on file extension
            var contentTypeProvider = new FileExtensionContentTypeProvider();
            if (!contentTypeProvider.TryGetContentType(file.Filename, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            return File(fileContents, contentType, file.Filename);
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
                        Content = Encoding.ASCII.GetString(stream.ToArray())
                    };

                    string apiUrl = "http://" + Environment.GetEnvironmentVariable("SC_IP") + ":8090/file";

                    using (HttpClient client = new HttpClient())
                    {
                        var content = new StringContent($"{{\"authtoken\":\"Test\",\"filename\":\"{newFile.Filename}\",\"filetype\":\"{newFile.Filetype}\",\"content\":\"{newFile.Content}\"}}", Encoding.UTF8, "application/json");
                        var response = await client.PostAsync(apiUrl,content);

                        if (response.IsSuccessStatusCode)
                        {
                            string stringResponse = await response.Content.ReadAsStringAsync();
                            //response: success ,message        message: message, fileid

                            Response<object> responseObject = await Response<object>.DeserializeJSON(stringResponse);

                            if (responseObject.Success)
                            {
                                Response<FileModel> file = await Response<FileModel>.DeserializeJSON(stringResponse);
                                ShowPopup(file.Message.Content);
                                var files = Files;
                                files.Add(file.Message);
                                Files = files;
                            }
                            else
                            {
                                ShowPopup("Response failed:" + responseObject.Message);
                            }

                        }
                        else
                        {
                            ShowPopup("Failure");
                        }

                    }
                }
            }
            else
            {
                ShowPopup("Invalid file upload");
            }

            return null;
        }

    }

}