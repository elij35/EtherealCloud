using Ethereal_Cloud.Helpers;
using Ethereal_Cloud.Models.Upload.Get.Folder;
using Ethereal_Cloud.Models.Upload.Get;
using Microsoft.AspNetCore.Mvc.RazorPages;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Ethereal_Cloud.Pages
{
    public class SharedModel : PageModel
    {
        //list of files to be shown to user
        public List<FolderContentDisplay> DisplayList = new List<FolderContentDisplay>();

        public bool sortDisplay = false;

        public bool sharedTab = true;


        public async Task OnGet()
        {
            sortDisplay = SortManagement.GetSorting(HttpContext);

            bool sharedWithMe = ShareManagement.GetActiveShare(HttpContext);

            // Shared with me request
            string endpointShare = "v2/file/share";

            if (!sharedWithMe)
            {
                // Sharing with request
                endpointShare = "temp";
            }

            Logger.LogToConsole(ViewData, "Endpoint: " + endpointShare);

            //Make request
            var response = await ApiRequestV2.Files(ViewData, HttpContext, endpointShare, true, null);


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
            bool sortAlpha = SortManagement.GetSorting(HttpContext);

            SortManagement.SetSorting(HttpContext, !sortAlpha);

            sortDisplay = !sortAlpha;

            Response.Redirect("/Shared");
        }








    }
}
