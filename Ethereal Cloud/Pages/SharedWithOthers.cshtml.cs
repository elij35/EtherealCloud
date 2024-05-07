using Ethereal_Cloud.Helpers;
using Ethereal_Cloud.Models;
using Ethereal_Cloud.Models.Upload.Get;
using Ethereal_Cloud.Models.Upload.Get.File;
using Ethereal_Cloud.Models.Upload.Get.Folder;
using Ethereal_Cloud.Models.Upload.Rename;
using Ethereal_Cloud.Models.Upload.Share;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Ethereal_Cloud.Pages
{
    public class SharedWithOthersModel : PageModel
    {
        //list of files to be shown to user
        public List<ShareContentDisplay> DisplayList = new List<ShareContentDisplay>();

        public bool sortDisplay = false;

        public bool sharedTab = true;


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

            bool sharedWithMe = CookieManagement.GetActiveShare(HttpContext);

            //Make request
            var response = await ApiRequestV2.Files(HttpContext, "v2/file/sharing", true, null);

            if (response != null)
            {
                //Put response in form of ShareContentRecieve
                string jsonString = response.ToString();
                List<ShareFileMetaRecieve>? folderContent = JsonSerializer.Deserialize<List<ShareFileMetaRecieve>>(jsonString);

                DisplayList = new List<ShareContentDisplay>();

                //Add files to display list
                foreach (ShareFileMetaRecieve file in folderContent)
                {

                    ShareContentDisplay newFile = new()
                    {
                        Id = file.FileID,
                        Name = file.Filename,
                        Type = file.Filetype,
                        SharingUsers = file.SharingUsers
                    };

                    DisplayList.Add(newFile);
                }


                //Sort list
                DisplayList = SortHelper.SortSharedDisplay(HttpContext, DisplayList);

            }
            else
            {

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
            var response = await ApiRequest.Files(HttpContext, "v1/file/" + fileId, dataObject);

            if (response != null)
            {
                string jsonString = response.ToString();
                FileModel file = JsonSerializer.Deserialize<FileModel>(jsonString);

                byte[] bytes = Convert.FromHexString(file.Content);

                return File(bytes, file.Filetype, file.Filename);
            }
            else
            {
                // Failure
                UserFeedbackMessage feedbackMessage = new UserFeedbackMessage
                {
                    ResultSuccess = false,
                    Message = "An error occured when trying to download a file. Please try again later."
                };

                TempData["UserFeedback"] = JsonConvert.SerializeObject(feedbackMessage);

                return RedirectToPage("/SharedWithOthers");
            }


        }


        public async Task OnPostDeleteAsync(string fileId, string type)
        {
            // Check fileId validity
            if (fileId == null || type == null)
            {
                // Failure
                UserFeedbackMessage feedbackMessage = new UserFeedbackMessage
                {
                    ResultSuccess = false,
                    Message = "An error occured when trying to delete a file. Please try again later."
                };

                TempData["UserFeedback"] = JsonConvert.SerializeObject(feedbackMessage);

                return;
            }


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
            var response = await ApiRequestV2.Files(HttpContext, "v2/" + uriFileType + "/remove/" + fileId, true, null);

            if (response != null)
            {
                // Success
                UserFeedbackMessage feedbackMessage = new UserFeedbackMessage
                {
                    ResultSuccess = true,
                    Message = "File was deleted."
                };

                TempData["UserFeedback"] = JsonConvert.SerializeObject(feedbackMessage);


                Response.Redirect("/SharedWithOthers");
            }
            else
            {
                // Failure
                UserFeedbackMessage feedbackMessage = new UserFeedbackMessage
                {
                    ResultSuccess = false,
                    Message = "An error occured when trying to delete a file. Please try again later."
                };

                TempData["UserFeedback"] = JsonConvert.SerializeObject(feedbackMessage);

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
            var response = await ApiRequestV2.Files(HttpContext, "v2/" + uriFileType + "/rename/" + renameDetails.Id, true, dataObject);

            if (response != null)
            {
                // Success
                UserFeedbackMessage feedbackMessage = new UserFeedbackMessage
                {
                    ResultSuccess = true,
                    Message = "File was renamed."
                };

                TempData["UserFeedback"] = JsonConvert.SerializeObject(feedbackMessage);

                Response.Redirect("/SharedWithOthers");
            }
            else
            {
                // Failure
                UserFeedbackMessage feedbackMessage = new UserFeedbackMessage
                {
                    ResultSuccess = false,
                    Message = "An error occured when trying to rename a file. Please try again later."
                };

                TempData["UserFeedback"] = JsonConvert.SerializeObject(feedbackMessage);


                Response.Redirect("/SharedWithOthers");
                return;
            }
        }


        public async Task OnPostShare(ShareDetails shareDetails)
        {
            //create file object
            var dataObject = new Dictionary<string, object?>
            {
                { "ShareUsername", shareDetails.Username }
            };



            //Make request
            var response = await ApiRequestV2.Files( HttpContext, "v2/file/share/" + shareDetails.Id, true, dataObject);

            if (response != null)
            {
                // Success
                UserFeedbackMessage feedbackMessage = new UserFeedbackMessage
                {
                    ResultSuccess = true,
                    Message = "File was shared"
                };

                TempData["UserFeedback"] = JsonConvert.SerializeObject(feedbackMessage);


                Response.Redirect("/SharedWithOthers");
            }
            else
            {
                // Failure
                UserFeedbackMessage feedbackMessage = new UserFeedbackMessage
                {
                    ResultSuccess = false,
                    Message = "An error occured when trying to share a file. Please try again later."
                };

                TempData["UserFeedback"] = JsonConvert.SerializeObject(feedbackMessage);


                Response.Redirect("/SharedWithOthers");
            }
        }

    }
}
