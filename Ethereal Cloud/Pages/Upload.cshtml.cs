using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.StaticFiles;
using System.Text;

namespace Ethereal_Cloud.Pages
{
    public class UploadModel : PageModel
    {
        [BindProperty]
        public string Username { get; set; }

        public List<FileModel> Files { get; set; }

        public async Task OnGetFileAsync(int Id)
        {
            string apiUrl = "http://" + Environment.GetEnvironmentVariable("SC_IP") + ":8090/file";
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string stringResponse = await response.Content.ReadAsStringAsync();

                    Response<FileModel> file = await Response<FileModel>.DeserializeJSON(stringResponse);

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

            byte[] fileContents = Convert.FromBase64String(file.Content);

            // Determine content type based on file extension
            var contentTypeProvider = new FileExtensionContentTypeProvider();
            if (!contentTypeProvider.TryGetContentType(file.Filename, out var contentType))
            {
                contentType = "application/octet-stream";
            }
            return File(fileContents, contentType, file.Filename);
        }

        //Uploading files to host
        public async Task<IActionResult> OnPostUploadAsync(IFormFile uploadedFile)
        {
            if (uploadedFile != null && uploadedFile.Length > 0)
            {
                using (var stream = new MemoryStream())
                {
                    await uploadedFile.CopyToAsync(stream);
                    var newFile = new FileModel
                    {
                        AuthToken = "TempToken",
                        Filename = uploadedFile.FileName,
                        Filetype = Path.GetExtension(uploadedFile.FileName),
                        Content = Convert.ToBase64String(stream.ToArray())
                    };

                    string apiUrl = "http://" + Environment.GetEnvironmentVariable("SC_IP") + ":8090/file";
                    using (HttpClient client = new HttpClient())
                    {
                        var content = new StringContent($"{{\"authtoken\":\"{newFile.AuthToken}\",\"filename\":\"{newFile.Filename}\",\"filetype\":\"{newFile.Filetype}\",\"content\":\"{newFile.Content}\"}}", Encoding.UTF8, "application/json");
                        var response = await client.PostAsync(apiUrl, content);

                        if (response.IsSuccessStatusCode)
                        {
                            string stringResponse = await response.Content.ReadAsStringAsync();

                            Response<FileModel> file = await Response<FileModel>.DeserializeJSON(stringResponse);
                            ShowPopup(file.Success + " : " + file.Message);
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
            return RedirectToPage("Index");
        }
    }
}