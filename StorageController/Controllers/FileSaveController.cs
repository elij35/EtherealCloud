using Microsoft.AspNetCore.Mvc;
using StorageController.Data.Models;
using StorageController.Data;
using System.IdentityModel.Tokens.Jwt;

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

        [Route("/file")]
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<string> SaveFile([FromBody] FileDataSave fileData)
        {

            // Checking the user's token
            JwtSecurityToken? token = await AuthManager.ValidateToken(fileData.AuthToken);

            if (token == null)
            {
                return await new Response<string>(false, "Invalid auth token.").Serialize();
            }

            int? userID = await AuthManager.GetUserIDFromToken(token);

            if (userID == null)
            {
                return await new Response<string>(false, "Invalid auth token.").Serialize();
            }

            // Getting the database
            DataHandler db = new DataHandler();
            FileData? lastFile = db.Files.OrderBy(file => file.FileID).LastOrDefault();

            int nextID = 1;

            if (lastFile != null)
                nextID = lastFile.FileID + 1;

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

            // Trying to save the file in the bucket
            Response<string> savedFile = (await BucketAPIHandler.SendFileContent(nextID, fileData.Content, fileSave.BucketLocation));

            // Saving file to database if the bucket save succeeded
            int success = 0;
            if (savedFile.Success)
            {
                await db.Files.AddAsync(fileSave);
                await db.SaveChangesAsync();

                userFileSave.File = fileSave;
                await db.UserFiles.AddAsync(userFileSave);
                success = await db.SaveChangesAsync();

            }

            if (success < 1)
                return await new Response<string>(false, "Could not save file.").Serialize();

            FileSaveInfo fileSaveInfo = new FileSaveInfo()
            {
                Message = "File Saved.",
                FileID = nextID
            };

            return await new Response<FileSaveInfo>(true, fileSaveInfo).Serialize();
        }

        public struct FolderSaveData
        {
            public string AuthToken { get; set; }
            public string FolderName { get; set; }
            public int? ParentFolder {  get; set; }

        }
    }
}
