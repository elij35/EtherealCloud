using Ethereal_Cloud.Models;
using Ethereal_Cloud.Pages;
using Microsoft.AspNetCore.StaticFiles;
using Newtonsoft.Json;
using System.Diagnostics.Metrics;
using System.Text;

namespace Ethereal_Cloud.Helpers
{
    public class FileUpload
    {
        public static async Task SendDataToApi(IEnumerable<IFormFile> uploadedFiles, string apiUrl, HttpContext context)
        {
            
            foreach (var file in uploadedFiles)
            {
                bool isDirectory = Directory.Exists(file.FileName);

                await SendToApi(file, apiUrl, isDirectory, context);

                if (isDirectory)
                {
                    await SendDataToApi(EnumerateFilesInDirectory(file.FileName), apiUrl, context);
                }
            }
            
        }

        static IEnumerable<IFormFile> EnumerateFilesInDirectory(string directoryPath)
        {
            foreach (var filePath in Directory.GetFiles(directoryPath))
            {
                var file = new FormFile(new MemoryStream(File.ReadAllBytes(filePath)), 0, 0, filePath, Path.GetFileName(filePath));
                yield return file;
            }
        }

        static async Task SendToApi(IFormFile uploadedFile, string apiUrl, bool isDirectory, HttpContext context)
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

                    string authToken = Helpers.AuthTokenManagement.GetToken(context);

                    using (HttpClient client = new HttpClient())
                    {
                        //create json object
                        var dataObject = new
                        {
                            authtoken = authToken,
                            filename = newFile.Filename,
                            filetype = newFile.Filetype,
                            content = newFile.Content,
                            folder = "1"
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
                                //ShowPopup("Response failed:" + responseObject.Message);
                            }

                        }
                        else
                        {
                            //ShowPopup("Failure");
                        }

                    }
                }
            }
            else
            {
                //ShowPopup("Invalid file upload");
            }

            //return null;
        }
    }
}

