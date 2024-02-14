using Microsoft.AspNetCore.Mvc;
using StorageController.Data.Models;
using StorageController.Data;
using System.IdentityModel.Tokens.Jwt;
using static StorageController.Controllers.FileGetController;

namespace StorageController.Controllers
{

    [ApiController]
    public class FileSaveController
    {

        public struct FileSaveInfo
        {
            public string Message { get; set; }
            public int FileID { get; set; }
        }

        public struct FileDataSave
        {
            public string AuthToken { get; set; }
            public string Filename { get; set; }
            public string Filetype { get; set; }
            public string Content { get; set; }
            public int? FolderID { get; set; }
        }

        [Route("/v1/file")]
        [HttpPost]
        [DisableRequestSizeLimit]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<string> SaveFile([FromBody] FileDataSave fileData)
        {

            Response<string> authResponse = await AuthManager.AuthorizeUser(fileData.AuthToken);

            if (!authResponse.Success)
                return await authResponse.Serialize();

            int userID = int.Parse(authResponse.Message);

            // Getting the database
            DataHandler db = new DataHandler();

            // Ensuring the user is valid
            User? userData = db.Users.FirstOrDefault(user => user.UserID == userID);
            if (userData == null)
            {
                return await new Response<string>(false, "Invalid user.").Serialize();
            }

            // Creating the file save data
            FileData fileSave = new();
            fileSave.FileName = fileData.Filename;
            fileSave.FileType = fileData.Filetype;

            // Setting it to save to the first bucket TEMP
            // TODO: Choose bucket to save to depending on bucket storage remaining, or randomly pick bucket
            fileSave.BucketLocation = db.Buckets.First();

            // Checking if a folder ID was supplied, and trying to retrieve it if so
            Folder? fileFolder = null;
            if (fileData.FolderID != null)
            {
                fileFolder = db.Folders.FirstOrDefault(folder => folder.FolderID == fileData.FolderID);

                if (fileFolder == null)
                    return await new Response<string>(false, "Invalid folder.").Serialize();
            }

            // Setting the file's folder
            fileSave.FolderData = fileFolder;

            // Creating user file link
            UserFile userFileSave = new();
            userFileSave.Privilege = "Owner";
            userFileSave.UserData = userData;

            FileData saved = (await db.Files.AddAsync(fileSave)).Entity;
            await db.SaveChangesAsync();

            userFileSave.File = fileSave;
            await db.UserFiles.AddAsync(userFileSave);
            int success = await db.SaveChangesAsync();

            if (success < 1)
            {
                Response<string> deletedFile = await BucketAPIHandler.DeleteFileContent(userFileSave.FileID, fileSave.BucketLocation);
                return await new Response<string>(false, $"Could not save file.").Serialize();
            }

            // Trying to save the file in the bucket
            Response<string> savedFile = (await BucketAPIHandler.SendFileContent(saved.FileID, fileData.Content, fileSave.BucketLocation));

            FileSaveInfo fileSaveInfo = new FileSaveInfo()
            {
                Message = "File Saved.",
                FileID = saved.FileID
            };

            return await new Response<FileSaveInfo>(true, fileSaveInfo).Serialize();
        }

        public struct FolderSaveData
        {
            public string AuthToken { get; set; }
            public string FolderName { get; set; }
            public int? ParentFolder {  get; set; }

        }

        public struct FolderReturnData
        {
            public int FolderID { get; set; }
            public string FolderName { get; set; }
        }

        [Route("/v1/folder")]
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<string> CreateFolder([FromBody] FolderSaveData folderSaveData)
        {

            Response<string> authResponse = await AuthManager.AuthorizeUser(folderSaveData.AuthToken);

            if (!authResponse.Success)
                return await authResponse.Serialize();

            int userID = int.Parse(authResponse.Message);

            DataHandler db = new();

            Folder? parentFolder = null;
            if (folderSaveData.ParentFolder != null)
            {
                parentFolder = db.Folders.FirstOrDefault(folder => folder.FolderID == folderSaveData.ParentFolder);

                if (parentFolder == null)
                {
                    return await new Response<string>(false, "Invalid folder.").Serialize();
                }

                UserFolder? userFolder = db.UserFolders.FirstOrDefault(userFolder => userFolder.UserID == userID && userFolder.FolderID == folderSaveData.ParentFolder);

                if (userFolder == null)
                {
                    return await new Response<string>(false, "Can't access this folder").Serialize();
                }
            }

            Folder folder = new();
            folder.ParentID = (parentFolder == null) ? null : parentFolder.FolderID;
            folder.FolderName = folderSaveData.FolderName;

            await db.Folders.AddAsync(folder);
            await db.SaveChangesAsync();

            UserFolder folderLink = new();
            folderLink.UserID = userID;
            folderLink.FolderID = folder.FolderID;
            folderLink.Privilege = "Owner";

            await db.UserFolders.AddAsync(folderLink);
            await db.SaveChangesAsync();

            FolderReturnData folderReturnData = new FolderReturnData()
            {
                FolderID = folder.FolderID,
                FolderName = folder.FolderName
            };

            return await new Response<FolderReturnData>(true, folderReturnData).Serialize();

        }

    }
}
