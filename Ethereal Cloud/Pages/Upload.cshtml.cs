using Ethereal_Cloud.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.StaticFiles;
using Newtonsoft.Json;
using System.Text;

namespace Ethereal_Cloud.Pages
{
    [DisableRequestSizeLimit] //Disables the file upload limit
    public class UploadModel : PageModel
    {
        [BindProperty]
        public string Username { get; set; }

        //list of files to be shown to user
        public List<FileInfo> Files;

        public async Task<IActionResult> OnGet()
        {
            
            bool fileFound = true;
            int counter = 1;
            while (fileFound) //TODO: THE LOOP WONT BE NEEDED AS A LIST SHOULD BE RETURNED
            {
                string apiUrl = "http://" + Environment.GetEnvironmentVariable("SC_IP") + ":8090/v1/file/" + counter;

                string authToken = Helpers.AuthTokenManagement.GetToken(HttpContext);

                using (HttpClient client = new HttpClient())
                {
                    //create json object
                    var dataObject = new
                    {
                        authtoken = authToken
                    };
                    var content = new StringContent(JsonConvert.SerializeObject(dataObject), Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        string stringResponse = await response.Content.ReadAsStringAsync();
                        Response<object> responseObject = await Response<object>.DeserializeJSON(stringResponse);

                        if (responseObject.Success)
                        {
                            Response<FileInfo> file = await Response<FileInfo>.DeserializeJSON(stringResponse); //THE MESSAGE SHOULD ONLY RETURN FILEINFO (FILENAME, FILETYPE) THE DATA IS GOTTEN ON DOWNLOAD
                            var files = Files;
                            files.Add(file.Message);
                            Files = files;
                        }
                        else
                        {
                            ShowPopup("Failure: " + responseObject.Message);
                            fileFound = false;
                        }

                    }
                    else
                    {
                        ShowPopup("Failure: " + response.Content);
                        fileFound = false;
                    }

                }


                counter++;
            }
            
            return Page();
        }

        private void ShowPopup(string status)
        {
            ViewData["PopupStatus"] = status;
        }



        public async Task<IActionResult> OnGetDownload(string filename)
        {
            var files = Files;

            var file = files?.FirstOrDefault(f => f.Filename == filename);

            if (file == null)
            {
                return NotFound();
            }

            string apiUrl = "http://" + Environment.GetEnvironmentVariable("SC_IP") + ":8090/v1/file/" + "1";//counter

            string authToken = Helpers.AuthTokenManagement.GetToken(HttpContext);

            using (HttpClient client = new HttpClient())
            {
                //create json object
                var dataObject = new
                {
                    authtoken = authToken,
                    filename = file.Filename
                };
                var content = new StringContent(JsonConvert.SerializeObject(dataObject), Encoding.UTF8, "application/json");

                var response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    string stringResponse = await response.Content.ReadAsStringAsync();
                    Response<object> responseObject = await Response<object>.DeserializeJSON(stringResponse);

                    if (responseObject.Success)
                    {
                        Response<string> fileContent = await Response<string>.DeserializeJSON(stringResponse); //message should only contain filecontent

                        byte[] fileContents = Convert.FromBase64String(fileContent.Message);

                        // Determine content type based on file extension
                        var contentTypeProvider = new FileExtensionContentTypeProvider();
                        if (!contentTypeProvider.TryGetContentType(file.Filename, out var contentType))
                        {
                            contentType = "application/octet-stream";
                        }

                        return File(fileContents, contentType, file.Filename);





                    }
                    else
                    {
                        ShowPopup("Failure: " + responseObject.Message);
                    }

                }
                else
                {
                    ShowPopup("Failure: " + response.Content);
                }

            }

            return null;
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

                    string apiUrl = "http://" + Environment.GetEnvironmentVariable("SC_IP") + ":8090/file";

                    string authToken = Helpers.AuthTokenManagement.GetToken(HttpContext);

                    using (HttpClient client = new HttpClient())
                    {
                        //create json object
                        var dataObject = new
                        {
                            authtoken = authToken,
                            filename = newFile.Filename,
                            filetype = newFile.Filetype,
                            content = newFile.Content,
                            folder = ""
                        };
                        var content = new StringContent(JsonConvert.SerializeObject(dataObject), Encoding.UTF8, "application/json");

                        var response = await client.PostAsync(apiUrl, content);

                        if (response.IsSuccessStatusCode)
                        {
                            string stringResponse = await response.Content.ReadAsStringAsync();
                            //response: success ,message        message: message, fileid

                            Response<object> responseObject = await Response<object>.DeserializeJSON(stringResponse);

                            if (!responseObject.Success)
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
            
            return Page();
        }

    }

}