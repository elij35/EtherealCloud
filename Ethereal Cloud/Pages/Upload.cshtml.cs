using Ethereal_Cloud.Helpers;
using Ethereal_Cloud.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Build.ObjectModelRemoting;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Primitives;
using NuGet.Packaging;
using System.Net;
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
        public List<FolderContentDisplay> DisplayList = new List<FolderContentDisplay>();

        public List<FolderDataRecieve> FolderPath = new List<FolderDataRecieve>();

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
            }
            
        }



        public async Task<IActionResult> OnGetDownload(string itemname)
        {
            Logger.LogToConsole(ViewData, "Hello");
            
            string[] fileData = itemname.Split(':');

            int fileId = int.Parse(fileData[0]);

            Logger.LogToConsole(ViewData, "Fileid: " + fileData[0]);
            
            
            //create object
            var dataObject = new Dictionary<string, object?>
            {
                { "authtoken", AuthTokenManagement.GetToken(HttpContext)}
            };

            //Make request
            var response = await ApiRequest.Files(ViewData, HttpContext, "v1/file/" + fileId, dataObject);

            if (response != null)
            {
                string jsonString = response.ToString();
                FileModel file = JsonSerializer.Deserialize<FileModel>(jsonString);

                Logger.LogToConsole(ViewData, "Successfull download: " + file.Content + " " + file.Filetype + " " + file.Filename);

                //Response.Redirect("/Upload");

                return File(Convert.FromBase64String(file.Content), file.Filetype, file.Filename);
            }
            else
            {
                Logger.LogToConsole(ViewData, "Fail: " + fileId);

                return RedirectToPage("/Upload");
            }
            
           
        }

        public async Task OnGetNavigate(string itemname)
        {
            //\
            string[] folderData = itemname.Split(':');


            var path = PathManagement.Get(HttpContext);

            List<FolderDataRecieve> folderPath = new List<FolderDataRecieve>();

            if (path != null)
            {
                folderPath = path;
            }

            FolderDataRecieve navigateTo = new()
            {
                FolderID = int.Parse(folderData[0]),
                Foldername = folderData[1]
            };

            folderPath.Add(navigateTo);

            var serializedFolderPath = JsonSerializer.Serialize(folderPath);

            PathManagement.Set(HttpContext, serializedFolderPath);

            Response.Redirect("/Upload");

        }

        public async Task OnGetGoToFolderInPathAsync(int folderId)
        {
            bool success = PathManagement.GoBackInFolderPath(HttpContext, folderId, ViewData);
            
            if (success)
            {
                Logger.LogToConsole(ViewData, "Navigated to folder with id: " + folderId);
                
            }
            else
            {
                Logger.LogToConsole(ViewData, "Failed navigation to: " + folderId);

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

                    

                    //create file object
                    var dataObject = new Dictionary<string, object?>
                    {
                        { "AuthToken", AuthTokenManagement.GetToken(HttpContext) },
                        { "Filename", uploadedFile.FileName },
                        { "Filetype", MimeType.GetMimeType(uploadedFile.FileName) },
                        { "Content", Convert.ToBase64String(stream.ToArray()) }
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
                        Logger.LogToConsole(ViewData, "Bad upload response");
                    }
                }
                
            }
            else
            {
                Logger.LogToConsole(ViewData, "Invalid file upload");
            }
            
        }


        public async Task OnGetCreatefolderAsync(string foldername)
        {
            //create file object
            var dataObject = new Dictionary<string, object?>
            {
                { "AuthToken", AuthTokenManagement.GetToken(HttpContext) }, 
                { "FolderName", foldername },
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
            }
            
        }



    }

}