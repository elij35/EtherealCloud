using Ethereal_Cloud.Helpers;
using Ethereal_Cloud.Models.Upload.Get.Folder;
using Ethereal_Cloud.Models.Upload.Get;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace Ethereal_Cloud.Pages
{
    public class BinModel : PageModel
    {
        //list of files to be shown to user
        public List<FolderContentDisplay> DisplayList = new List<FolderContentDisplay>();

        public async Task OnGet()
        {
            //Make request
            var response = await ApiRequestV2.Files(ViewData, HttpContext, "v2/bin", true, null);

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

                Logger.LogToConsole(ViewData, "Successful get of files: " + JsonSerializer.Serialize(DisplayList));
            }
            else
            {
                Logger.LogToConsole(ViewData, "Failed Get");

                ViewData["FailureMessage"] = "Failed to get files & folders. Please try again.";
            }
        }
    }
}
