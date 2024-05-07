using Ethereal_Cloud.Helpers;
using Ethereal_Cloud.Models;
using Ethereal_Cloud.Models.Upload.CreateFolder;
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
    [DisableRequestSizeLimit] //Disables the file upload limit
    public class UploadModel : PageModel
    {
        //list of files to be shown to user
        public List<FolderContentDisplay> DisplayList = new List<FolderContentDisplay>();

        public List<FolderDataRecieve> FolderPath = new List<FolderDataRecieve>();

        public bool sortDisplay = false;

        public void FolderPathForDisplay()
        {
            //Sets the filepath list to be displayed on the interface
            FolderPath = CookieManagement.GetFolderPath(HttpContext);
        }

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

            FolderPathForDisplay();

            int? folderId = PathManagement.GetCurrentFolderId(HttpContext);


            //Make request
            var response = await ApiRequestV2.Files(HttpContext, "v2/folder/files/" + folderId, true, null);

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

                ViewData["FailureMessage"] = "Failed to get files & folders. Please try again.";
            }
        }

        public async Task OnPostSort()
        {
            bool sortAlpha = CookieManagement.GetSorting(HttpContext);

            CookieManagement.SetCookie(HttpContext, "Sort", (!sortAlpha).ToString());

            sortDisplay = !sortAlpha;

            Response.Redirect("/Upload");
        }


        public async Task<IActionResult> OnPostDownload(int fileId)
        {
            //create object
            var dataObject = new Dictionary<string, object?>
            {
                { "authtoken", CookieManagement.GetAuthToken(HttpContext)}
            };

            //Make request
            var response = await ApiRequest.Files( HttpContext, "v1/file/" + fileId, dataObject);

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

                return RedirectToPage("/Upload");
            }


        }

        public async Task OnPostNavigate(int Id, string Name)
        {
            var path = CookieManagement.GetFolderPath(HttpContext);

            List<FolderDataRecieve> folderPath = new List<FolderDataRecieve>();

            if (path != null)
            {
                folderPath = path;
            }

            FolderDataRecieve navigateTo = new()
            {
                FolderID = Id,
                Foldername = Name
            };

            folderPath.Add(navigateTo);

            var serializedFolderPath = JsonSerializer.Serialize(folderPath);

            CookieManagement.SetCookie(HttpContext, "FolderPath", serializedFolderPath);

            Response.Redirect("/Upload");

        }

        public async Task OnGetGoToFolderInPathAsync(GotoDetails details)
        {
            PathManagement.GoBackInFolderPath(HttpContext, details.Id);

            Response.Redirect("/Upload");

        }



        public async Task OnPostUploadAsync(List<IFormFile> uploadedFiles)
        {
            foreach (IFormFile singleFile in uploadedFiles)
            {
                if (singleFile != null && singleFile.Length > 0)
                {
                    using (var stream = new MemoryStream())
                    {
                        //Where folder the user is in
                        int? currentFolder = PathManagement.GetCurrentFolderId(HttpContext);

                        //Hold the file contents in the stream
                        await singleFile.CopyToAsync(stream);

                        byte[] bytes = stream.ToArray();
                        string hexBytes = Convert.ToHexString(bytes);

                        //create file object
                        var dataObject = new Dictionary<string, object?>
                        {
                            { "AuthToken", CookieManagement.GetAuthToken(HttpContext) },
                            { "Filename", singleFile.FileName },
                            { "Filetype", MimeType.GetMimeType(singleFile.FileName) },
                            { "Content", hexBytes }
                        };
                        //If the user isnt in the root
                        if (currentFolder != null)
                        {
                            dataObject.Add("FolderId", currentFolder);
                        }


                        //Make request
                        var response = await ApiRequest.Files(HttpContext, "v1/file", dataObject);

                        if (response != null)
                        {
                            // Success
                            UserFeedbackMessage feedbackMessage = new UserFeedbackMessage
                            {
                                ResultSuccess = true,
                                Message = "File(s) were uploaded."
                            };

                            TempData["UserFeedback"] = JsonConvert.SerializeObject(feedbackMessage);
                        }
                        else
                        {
                            // Failure
                            UserFeedbackMessage feedbackMessage = new UserFeedbackMessage
                            {
                                ResultSuccess = false,
                                Message = "An error occured when trying to upload file(s). Please try again later."
                            };

                            TempData["UserFeedback"] = JsonConvert.SerializeObject(feedbackMessage);
                        }
                    }

                }
                else
                {
                    // Invalid File
                    UserFeedbackMessage feedbackMessage = new UserFeedbackMessage
                    {
                        ResultSuccess = false,
                        Message = "File contained no data to upload."
                    };

                    TempData["UserFeedback"] = JsonConvert.SerializeObject(feedbackMessage);
                }
            }



            Response.Redirect("/Upload");

        }



        [BindProperty]
        public CreateFolderDetails createFolderDetails { get; set; }

        public async Task OnPostCreateFolderAsync()
        {
            if (!ModelState.IsValid)
            {
                UserFeedbackMessage feedbackMessage = new UserFeedbackMessage
                {
                    ResultSuccess = false,
                    Message = "Could not create a folder with that name."
                };

                TempData["UserFeedback"] = JsonConvert.SerializeObject(feedbackMessage);

                Response.Redirect("/Upload");
                return;
            }

            //create file object
            var dataObject = new Dictionary<string, object?>
            {
                { "AuthToken", CookieManagement.GetAuthToken(HttpContext) },
                { "FolderName", createFolderDetails.FolderName },
                { "ParentFolder", PathManagement.GetCurrentFolderId(HttpContext) }
            };

            //Make request
            var response = await ApiRequest.Files(HttpContext, "v1/folder", dataObject);

            if (response != null)
            {
                // Success
                UserFeedbackMessage feedbackMessage = new UserFeedbackMessage
                {
                    ResultSuccess = true,
                    Message = "Folder has been created"
                };

                TempData["UserFeedback"] = JsonConvert.SerializeObject(feedbackMessage);

            }
            else
            {
                // Failure
                UserFeedbackMessage feedbackMessage = new UserFeedbackMessage
                {
                    ResultSuccess = false,
                    Message = "An error occured when trying to create a folder. Please try again later."
                };

                TempData["UserFeedback"] = JsonConvert.SerializeObject(feedbackMessage);

            }

            Response.Redirect("/Upload");

        }


        public async Task OnPostDeleteAsync(string fileId, string type)
        {
            // Check fileId validity
            if (fileId == null || type == null)
            {
                UserFeedbackMessage feedbackMessage = new UserFeedbackMessage
                {
                    ResultSuccess = false,
                    Message = "An error occured when trying to delete. Please try again later."
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
                    Message = uriFileType + " was deleted."
                };

                TempData["UserFeedback"] = JsonConvert.SerializeObject(feedbackMessage);

                Response.Redirect("/Upload");
            }
            else
            {
                UserFeedbackMessage feedbackMessage = new UserFeedbackMessage
                {
                    ResultSuccess = false,
                    Message = "An error occured when trying to delete. Please try again later."
                };

                TempData["UserFeedback"] = JsonConvert.SerializeObject(feedbackMessage);

                Response.Redirect("/Upload");
                return;
            }
        }



        public async Task OnPostShare(ShareDetails shareDetails)
        {

            if (shareDetails.Username == null || shareDetails.Username.Trim() == "")
            {
                UserFeedbackMessage feedbackMessage = new UserFeedbackMessage
                {
                    ResultSuccess = false,
                    Message = "An error occured when trying to share a file. Please try again later."
                };

                TempData["UserFeedback"] = JsonConvert.SerializeObject(feedbackMessage);

                Response.Redirect("/Upload");
                return;
            }

            //create file object
            var dataObject = new Dictionary<string, object?>
            {
                { "ShareUsername", shareDetails.Username }
            };



            //Make request
            var response = await ApiRequestV2.Files(HttpContext, "v2/file/share/" + shareDetails.Id, true, dataObject);

            if (response != null)
            {
                // Success
                UserFeedbackMessage feedbackMessage = new UserFeedbackMessage
                {
                    ResultSuccess = true,
                    Message = "File was shared"
                };

                TempData["UserFeedback"] = JsonConvert.SerializeObject(feedbackMessage);

                Response.Redirect("/Upload");
            }
            else
            {
                UserFeedbackMessage feedbackMessage = new UserFeedbackMessage
                {
                    ResultSuccess = false,
                    Message = "An error occured when trying to share a file. Please try again later."
                };

                TempData["UserFeedback"] = JsonConvert.SerializeObject(feedbackMessage);

                Response.Redirect("/Upload");
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
            var response = await ApiRequestV2.Files( HttpContext, "v2/" + uriFileType + "/rename/" + renameDetails.Id, true, dataObject);

            if (response != null)
            {
                // Success
                UserFeedbackMessage feedbackMessage = new UserFeedbackMessage
                {
                    ResultSuccess = true,
                    Message = uriFileType + " was renamed."
                };

                TempData["UserFeedback"] = JsonConvert.SerializeObject(feedbackMessage);

                Response.Redirect("/Upload");
            }
            else
            {
                // Failure
                UserFeedbackMessage feedbackMessage = new UserFeedbackMessage
                {
                    ResultSuccess = false,
                    Message = "An error occured when trying to rename. Please try again later."
                };

                TempData["UserFeedback"] = JsonConvert.SerializeObject(feedbackMessage);

                Response.Redirect("/Upload");
                return;
            }
        }




    }
}