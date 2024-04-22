using Ethereal_Cloud.Helpers;
using Ethereal_Cloud.Models.Upload.Get;
using Ethereal_Cloud.Models.Upload.Get.Folder;
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

            Response.Redirect("/Shared");
        }
    }
}
