using Ethereal_Cloud.Helpers;
using Ethereal_Cloud.Models.Upload.Get;
using Ethereal_Cloud.Models.Upload.Get.File;
using Ethereal_Cloud.Models.Upload.Get.Folder;
using Ethereal_Cloud.Models.Upload.Rename;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Ethereal_Cloud.Pages
{
    public class SharedWithOthersModel : PageModel
    {
        //list of files to be shown to user
        public List<FolderContentDisplay> DisplayList = new List<FolderContentDisplay>();

        public bool sortDisplay = false;

        public bool sharedTab = true;


        //FileMetaRecieve[]?

        public async Task OnGet()
        {
            sortDisplay = CookieManagement.GetSorting(HttpContext);

            bool sharedWithMe = CookieManagement.GetActiveShare(HttpContext);

            // Shared with me request
            string endpointShare = "v2/file/sharing";

            if (!sharedWithMe)
            {
                // Sharing with request
                endpointShare = "temp";
            }

            Logger.LogToConsole(ViewData, "Endpoint: " + endpointShare);

            //Make request
            var response = await ApiRequestV2.Files(ViewData, HttpContext, "v2/file/sharing", true, null);


            if (response != null)
            {
                //Put response in form of ShareContentRecieve
                string jsonString = response.ToString();
                List<FileMetaRecieve>? folderContent = JsonSerializer.Deserialize<List<FileMetaRecieve>>(jsonString);

                DisplayList = new List<FolderContentDisplay>();

                //Add files to display list
                foreach (FileMetaRecieve file in folderContent)
                {

                    FolderContentDisplay newFile = new()
                    {
                        Id = file.FileID,
                        Name = file.Filename,
                        Type = file.Filetype
                    };

                    DisplayList.Add(newFile);
                }


                //Sort list
                DisplayList = SortHelper.SortDisplay(HttpContext, DisplayList);

            }
            else
            {
                //Logger.LogToConsole(ViewData, "Failed Get");

                ViewData["FailureMessage"] = "Failed to get files & folders. Please try again.";
            }

        }

        public async Task OnPostSort()
        {
            bool sortAlpha = CookieManagement.GetSorting(HttpContext);

            CookieManagement.SetCookie(HttpContext, "Sort", (!sortAlpha).ToString());

            sortDisplay = !sortAlpha;

            Response.Redirect("/SharedWithOthers");
        }


        public async Task<IActionResult> OnPostDownload(int fileId)
        {
            //create object
            var dataObject = new Dictionary<string, object?>
            {
                { "authtoken", CookieManagement.GetAuthToken(HttpContext)}
            };

            //Make request
            var response = await ApiRequest.Files(ViewData, HttpContext, "v1/file/" + fileId, dataObject);

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
                Logger.LogToConsole(ViewData, "Fail: " + fileId);

                return RedirectToPage("/SharedWithOthers");
            }


        }


        public async Task OnPostDeleteAsync(string fileId, string type)
        {
            // Check fileId validity
            if (fileId == null || type == null)
            {
                Logger.LogToConsole(ViewData, "Invalid: Model error");
                return;
            }




            Logger.LogToConsole(ViewData, "Deleted: " + fileId + type);

            string uriFileType;
            if (type.ToLower() == "folder")
            {
                uriFileType = "folder";
            }
            else
            {
                uriFileType = "file";
            }

            //Make request
            var response = await ApiRequestV2.Files(ViewData, HttpContext, "v2/" + uriFileType + "/remove/" + fileId, true, null);

            if (response != null)
            {
                //Logger.LogToConsole(ViewData, "Successfull Deletion: " + fileId + " : " + type);
                Logger.LogToConsole(ViewData, "After: " + fileId + type);
                Response.Redirect("/SharedWithOthers");
            }
            else
            {
                //Logger.LogToConsole(ViewData, "Bad delete response");
                Response.Redirect("/SharedWithOthers");
                return;
            }
        }


        public async Task OnPostRename(RenameDetails renameDetails)
        {

            //create file object
            var dataObject = new Dictionary<string, object?>
            {
                { "Name", renameDetails.Name }
            };

            string uriFileType;
            if (renameDetails.Type.ToLower() == "folder")
            {
                uriFileType = "folder";
            }
            else
            {
                uriFileType = "file";
            }


            //Make request
            var response = await ApiRequestV2.Files(ViewData, HttpContext, "v2/" + uriFileType + "/rename/" + renameDetails.Id, true, dataObject);

            if (response != null)
            {
                Logger.LogToConsole(ViewData, "Successfull Share: " + renameDetails.Id);
                Response.Redirect("/SharedWithOthers");
            }
            else
            {
                Logger.LogToConsole(ViewData, "Bad share response");
                Response.Redirect("/SharedWithOthers");
                return;
            }
        }

    }
}
