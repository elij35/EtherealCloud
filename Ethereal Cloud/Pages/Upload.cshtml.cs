using Ethereal_Cloud.Helpers;
using Ethereal_Cloud.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.StaticFiles;
using Newtonsoft.Json;
using NuGet.Protocol;
using System.CodeDom;
using System.IO;
using System.Text;
using System.Text.Json;
using static NuGet.Packaging.PackagingConstants;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Ethereal_Cloud.Pages
{
    [DisableRequestSizeLimit] //Disables the file upload limit
    public class UploadModel : PageModel
    {
        [BindProperty]
        public string Username { get; set; }

        //list of files to be shown to user
        public List<FolderContentDisplay> DisplayList;

        public async Task<IActionResult> OnGet(int? folderId)
        {
            //users current folder location
            folderId = null;

            //create object
            var dataObject = new Dictionary<string, object?>
            {
                { "authtoken", AuthTokenManagement.GetToken(HttpContext)}
            };
            
            //Make request
            var response = await ApiRequest.Files(ViewData, HttpContext, "v1/folder/files/" + folderId, dataObject);
            
            if (response != null)
            {
                //Put response in form of FolderContentRecieve
                string jsonString = response.ToString();
                FolderContentRecieve folderContent = JsonSerializer.Deserialize<FolderContentRecieve>(jsonString);

                //Add folders to display list
                foreach(FolderDataRecieve folder in folderContent.Folders)
                {
                    FolderContentDisplay newFolder = new()
                    {
                        Id = folder.FolderId,
                        Name = folder.FolderName,
                        Type = "Folder"
                    };

                    DisplayList.Add(newFolder);
                }

                //Add files to display list
                foreach (FileMetaRecieve file in folderContent.Files)
                {
                    FolderContentDisplay newFile = new()
                    {
                        Id = file.FileId,
                        Name = file.Filename,
                        Type = file.Filetype
                    };

                    DisplayList.Add(newFile);
                }

                
                Logger.LogToConsole(ViewData, "Successful get of files");
                //Reload the page to refresh files get
                return Page();
            }
            else
            {
                Logger.LogToConsole(ViewData, "Failed Get");
                return null;
            }
            
        }

        public async Task<IActionResult> OnGetDownload(string filename)
        {
            /*
            var file = DisplayList?.FirstOrDefault(f => f.Filename == filename);

            if (file == null)
            {
                Logger.LogToConsole(ViewData, "Cant find file to download");
                return NotFound();
            }

            var fileId = 1;

            //create object
            var dataObject = new Dictionary<string, object?>
            {
                { "authtoken", AuthTokenManagement.GetToken(HttpContext)}
            };

            //Make request
            var response = await ApiRequest.Files(ViewData, HttpContext, "v1/file/" + fileId, dataObject);

            if (response != null)
            {
                byte[] fileContents = Convert.FromBase64String((string)response);

                // Determine content type based on file extension
                var contentTypeProvider = new FileExtensionContentTypeProvider();
                if (!contentTypeProvider.TryGetContentType(file.Filename, out var contentType))
                {
                    contentType = "application/octet-stream";
                }

                Logger.LogToConsole(ViewData, "Successfull download of " + file.Filename);
                return File(fileContents, contentType, file.Filename);
            }
            else
            {
                return null;
            }
            */
            return null;
        }


        public async Task<IActionResult> OnPostUploadAsync(IFormFile uploadedFile)
        {
            if (uploadedFile != null && uploadedFile.Length > 0)
            {
                using (var stream = new MemoryStream())
                {
                    //Where folder the user is in
                    int? currentFolder = null;

                    //create file object
                    var dataObject = new Dictionary<string, object?>
                    {
                        { "authtoken", AuthTokenManagement.GetToken(HttpContext)},
                        { "filename", uploadedFile.FileName },
                        { "filetype", Path.GetExtension(uploadedFile.FileName) },
                        { "content", Convert.ToBase64String(stream.ToArray()) },
                        { "folder", currentFolder }
                    };

                    //Make request
                    var response = await ApiRequest.Files(ViewData, HttpContext, "v1/file",  dataObject);

                    if (response != null)
                    {
                        Logger.LogToConsole(ViewData, "Successfull file upload: " + response);
                        //Reload the page to refresh files get
                        return Page();
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            else
            {
                Logger.LogToConsole(ViewData, "Invalid file upload");
            }
            
            return null;
        }

    }

}