using Ethereal_Cloud.Helpers;
using Ethereal_Cloud.Models;
using Ethereal_Cloud.Models.Upload.Get;
using Ethereal_Cloud.Models.Upload.Get.Folder;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace Ethereal_Cloud.Pages
{
    public class BinModel : PageModel
    {
        //list of files to be shown to user
        public List<FolderContentDisplay> DisplayList = new List<FolderContentDisplay>();

        public bool sortDisplay = false;

        public async Task OnGet()
        {
            // Send user feedback
            if (TempData.ContainsKey("UserFeedback"))
            {
                UserFeedbackMessage message = JsonConvert.DeserializeObject<UserFeedbackMessage>(TempData["UserFeedback"].ToString());

                if (message.ResultSuccess)
                {
                    ViewData["SuccessMessage"] = message.Message;
                }
                else
                {
                    ViewData["FailureMessage"] = message.Message;
                }

            }


            sortDisplay = CookieManagement.GetSorting(HttpContext);

            //Make request
            var response = await ApiRequestV2.Files(HttpContext, "v2/bin", true, null);

            if (response != null)
            {
                //Put response in form of FolderContentRecieve
                string jsonString = response.ToString();
                FolderContentRecieve folderContent = System.Text.Json.JsonSerializer.Deserialize<FolderContentRecieve>(jsonString);

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
                // Failed to get files

                ViewData["FailureMessage"] = "Failed to get files & folders. Please try again.";
            }
        }

        public async Task OnPostSort()
        {
            bool sortAlpha = CookieManagement.GetSorting(HttpContext);

            CookieManagement.SetCookie(HttpContext, "Sort", (!sortAlpha).ToString());

            sortDisplay = !sortAlpha;

            Response.Redirect("/Bin");
        }


        public async Task OnPostRestoreAsync(string Id, string fileType)
        {
            // Check fileId validity
            if (Id == null || fileType == null)
            {
                // Failure
                UserFeedbackMessage feedbackMessage = new UserFeedbackMessage
                {
                    ResultSuccess = false,
                    Message = "An error occured when trying to restore. Please try again later."
                };

                TempData["UserFeedback"] = JsonConvert.SerializeObject(feedbackMessage);

                return;
            }



            string uriFileType;
            if (fileType.ToLower() == "folder")
            {
                uriFileType = "folder";
            }
            else
            {
                uriFileType = "file";
            }

            //Make request
            var response = await ApiRequestV2.Files( HttpContext, "v2/" + uriFileType + "/restore/" + Id, true, null);

            if (response != null)
            {
                // Valid
                // Success
                UserFeedbackMessage feedbackMessage = new UserFeedbackMessage
                {
                    ResultSuccess = true,
                    Message = uriFileType + " was restored."
                };

                TempData["UserFeedback"] = JsonConvert.SerializeObject(feedbackMessage);


                Response.Redirect("/Bin");
            }
            else
            {
                // Invalid
                // Failure
                UserFeedbackMessage feedbackMessage = new UserFeedbackMessage
                {
                    ResultSuccess = false,
                    Message = "An error occured when trying to restore. Please try again later."
                };

                TempData["UserFeedback"] = JsonConvert.SerializeObject(feedbackMessage);


                Response.Redirect("/Bin");
                return;
            }
        }


    }
}
