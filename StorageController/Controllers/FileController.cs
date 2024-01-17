using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using StorageController.Data;
using StorageController.Data.Models;
using System.Buffers.Text;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using static StorageController.Controllers.FileController;

namespace StorageController.Controllers
{

    [ApiController]
    [Route("/file")]
    public class FileController : Controller
    {

        public struct FileDataReturn
        {
            public string Filename { get; set; }
            public string Filetype { get; set; }
            public string Content { get; set; }
        }

        public struct FileDataSave
        {
            public string AuthToken { get; set; }
            public string Filename { get; set; }
            public string Filetype { get; set; }
            public string Content { get; set; }
            public bool Folder { get; set; }
        }

        public struct FileRequest
        {
            public string AuthToken { get; set; }

        }

        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [Route("/file/{id}")]
        public async Task<string> GetFile([FromBody] FileRequest fileRequest, [FromRoute] int id)
        {

            JwtSecurityToken? token = await AuthManager.ValidateToken(fileRequest.AuthToken);

            if (token == null)
            {
                return await new Response<string>(false, "Invalid auth token.").Serialize();
            }

            int? userID = await AuthManager.GetUserIDFromToken(token);

            if (userID == null)
            {
                return await new Response<string>(false, "Invalid auth token.").Serialize();
            }

            DataHandler db = new();

            FileData? file = db.Files.FirstOrDefault(file => file.FileID == id);

            if (file == null)
            {
                return await new Response<string>(false, "Invalid file.").Serialize();
            }

            UserFile? userFile = db.UserFiles.FirstOrDefault(userFile => userFile.FileID == id && userFile.UserID == userID);

            if (userFile == null) 
            {
                return await new Response<string>(false, "You don't have access to this file").Serialize();
            }

            Response<string> fileContent = await BucketAPIHandler.GetFileContent(id);

            if (!fileContent.Success)
                return await new Response<string>(false, "Cannot find file contents").Serialize();

            FileDataReturn fileData = new FileDataReturn
            {
                Filename = file.FileName,
                Filetype = file.FileType,
                Content = fileContent.Message
            };

            return await new Response<FileDataReturn>(true, fileData).Serialize();

        }

        public struct FileSaveInfo
        {
            public string Message { get; set; }
            public int FileID { get; set; }
        }

        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<string> SaveFile([FromBody] FileDataSave fileData)
        {

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

            DataHandler db = new DataHandler();
            int nextID = db.Files.Count() + 1;

            User? userData = db.Users.FirstOrDefault(user => user.UserID == userID);
            if (userData == null)
            {
                return await new Response<string>(false, "Invalid user.").Serialize();
            }

            FileData fileSave = new();
            fileSave.FileName = fileData.Filename;
            fileSave.FileType = fileData.Filetype;
            fileSave.BucketLocation = db.Buckets.First();

            UserFile userFileSave = new();
            userFileSave.Privilege = "Owner";
            userFileSave.UserData = userData;
               

            Response<string> savedFile = (await BucketAPIHandler.SendFileContent(nextID, fileData.Content));

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
    }
}
