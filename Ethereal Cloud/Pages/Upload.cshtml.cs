using Ethereal_Cloud.Helpers;
using Ethereal_Cloud.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Ethereal_Cloud.Pages
{
    [DisableRequestSizeLimit] //Disables the file upload limit
    public class UploadModel : PageModel
    {
        //list of files to be shown to user
        public List<FolderContentDisplay> DisplayList = new List<FolderContentDisplay>();

        public List<FolderDataRecieve> FolderPath = new List<FolderDataRecieve>();

        public int errornum = -1;

        public void FolderPathForDisplay()
        {
            //Sets the filepath list to be displayed on the interface
            FolderPath = PathManagement.Get(HttpContext);
        }

        public async Task OnGet()
        {
            FolderPathForDisplay();

            int? folderId = PathManagement.GetCurrentFolderId(HttpContext);

            //Logger.LogToConsole(ViewData, "Current Folder: " + folderId);

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

                DisplayList = new List<FolderContentDisplay>();

                //Add folders to display list
                foreach (FolderDataRecieve folder in folderContent.Folders)
                {
                    FolderContentDisplay newFolder = new()
                    {
                        Id = folder.FolderID,
                        Name = folder.Foldername,
                        Type = "Folder"
                    };

                    DisplayList.Add(newFolder);
                }

                //Add files to display list
                foreach (FileMetaRecieve file in folderContent.Files)
                {

                    FolderContentDisplay newFile = new()
                    {
                        Id = file.FileID,
                        Name = file.Filename,
                        Type = file.Filetype
                    };

                    DisplayList.Add(newFile);
                }

                //Logger.LogToConsole(ViewData, "Successful get of files: " + JsonSerializer.Serialize(DisplayList) + " FolderId: " + folderId);
            }
            else
            {
                Logger.LogToConsole(ViewData, "Failed Get");
                errornum = 0;
            }
        }


        public async Task<IActionResult> OnGetDownload(DownNavDetails details)
        {
            if (!ModelState.IsValid)
            {
                Logger.LogToConsole(ViewData, "Invalid: Model error");
                return RedirectToPage("/Upload");
            }


            //create object
            var dataObject = new Dictionary<string, object?>
            {
                { "authtoken", AuthTokenManagement.GetToken(HttpContext)}
            };

            //Make request
            var response = await ApiRequest.Files(ViewData, HttpContext, "v1/file/" + details.Id, dataObject);

            if (response != null)
            {
                string jsonString = response.ToString();
                FileModel file = JsonSerializer.Deserialize<FileModel>(jsonString);

                Logger.LogToConsole(ViewData, "Successfull download: " + file.Content + " " + file.Filetype + " " + file.Filename);

                //Response.Redirect("/Upload");

                byte[] bytes = Convert.FromHexString(file.Content);

                return File(bytes, file.Filetype, file.Filename);
            }
            else
            {
                Logger.LogToConsole(ViewData, "Fail: " + details.Id);

                return RedirectToPage("/Upload");
            }


        }

        public async Task OnGetNavigate(DownNavDetails details)
        {
            if (!ModelState.IsValid)
            {
                Logger.LogToConsole(ViewData, "Invalid: Model error");
                return;
            }


            var path = PathManagement.Get(HttpContext);

            List<FolderDataRecieve> folderPath = new List<FolderDataRecieve>();

            if (path != null)
            {
                folderPath = path;
            }

            FolderDataRecieve navigateTo = new()
            {
                FolderID = details.Id,
                Foldername = details.Name
            };

            folderPath.Add(navigateTo);

            var serializedFolderPath = JsonSerializer.Serialize(folderPath);

            PathManagement.Set(HttpContext, serializedFolderPath);

            Response.Redirect("/Upload");

        }

        public async Task OnGetGoToFolderInPathAsync(GotoDetails details)
        {
            bool success = PathManagement.GoBackInFolderPath(HttpContext, details.Id, ViewData);

            if (success)
            {
                Logger.LogToConsole(ViewData, "Navigated to folder with id: " + details.Id);

            }
            else
            {
                Logger.LogToConsole(ViewData, "Failed navigation to: " + details.Id);

            }

            Response.Redirect("/Upload");

        }



        public async Task OnPostUploadAsync(IFormFile uploadedFile)
        {
            if (uploadedFile != null && uploadedFile.Length > 0)
            {
                using (var stream = new MemoryStream())
                {
                    //Where folder the user is in
                    int? currentFolder = PathManagement.GetCurrentFolderId(HttpContext);

                    //Hold the file contents in the stream
                    await uploadedFile.CopyToAsync(stream);

                    byte[] bytes = stream.ToArray();
                    string hexBytes = Convert.ToHexString(bytes);

                    //create file object
                    var dataObject = new Dictionary<string, object?>
                    {
                        { "AuthToken", AuthTokenManagement.GetToken(HttpContext) },
                        { "Filename", uploadedFile.FileName },
                        { "Filetype", MimeType.GetMimeType(uploadedFile.FileName) },
                        { "Content", hexBytes }
                    };
                    //If the user isnt in the root
                    if (currentFolder != null)
                    {
                        dataObject.Add("FolderId", currentFolder);
                    }

                    
                    //Make request
                    var response = await ApiRequest.Files(ViewData, HttpContext, "v1/file", dataObject);

                    if (response != null)
                    {
                        Logger.LogToConsole(ViewData, "Successfull file upload: " + response);
                        Response.Redirect("/Upload");
                    }
                    else
                    {
                        //Logger.LogToConsole(ViewData, "Bad upload response");
                    }
                }

            }
            else
            {
                Logger.LogToConsole(ViewData, "Invalid file upload");
            }

        }



        [BindProperty]
        public CreateFolderDetails createFolderDetails { get; set; }

        public async Task OnPostCreateFolderAsync()
        {
            if (!ModelState.IsValid)
            {
                Logger.LogToConsole(ViewData, "Invalid: Model error");
                return;
            }

            //create file object
            var dataObject = new Dictionary<string, object?>
            {
                { "AuthToken", AuthTokenManagement.GetToken(HttpContext) },
                { "FolderName", createFolderDetails.FolderName },
                { "ParentFolder", PathManagement.GetCurrentFolderId(HttpContext) }
            };

            //Make request
            var response = await ApiRequest.Files(ViewData, HttpContext, "v1/folder", dataObject);

            if (response != null)
            {
                Logger.LogToConsole(ViewData, "Successfull folder creation: " + response);
                Response.Redirect("/Upload");
            }
            else
            {
                Logger.LogToConsole(ViewData, "Bad folder response");
                return;
            }

        }



    }

}