using Microsoft.AspNetCore.Mvc;
using StorageController.Data.Models;
using StorageController.Data;
using System.IdentityModel.Tokens.Jwt;

namespace StorageController.Controllers
{

    [ApiController]
    public class FileGetController
    {

        public struct FileDataReturn
        {
            public int FileID { get; set; }
            public string Filename { get; set; }
            public string Filetype { get; set; }
            public string Content { get; set; }
        }

        public struct FileMetaReturn
        {
            public int FileID { get; set; }
            public string Filename { get; set; }
            public string Filetype { get; set; }
        }

        public struct FolderDataReturn
        {
            public int FolderID { get; set; }
            public string Foldername { get; set; }
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

            Response<string> fileContent = await BucketAPIHandler.GetFileContent(id, file.BucketLocation);

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


    }
}
