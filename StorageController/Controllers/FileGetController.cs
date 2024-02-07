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
        [Route("/v1/file/{id}")]
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

            Bucket? bucket = db.Buckets.FirstOrDefault(bucket => bucket.BucketID == file.BucketID);

            if (bucket == null)
            {
                return await new Response<string>(false, "Invalid bucket.").Serialize();
            }

            Response<string> fileContent = await BucketAPIHandler.GetFileContent(id, bucket);

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

        public struct FolderContentReturn
        {
            public FolderDataReturn[] Folders { get; set; }
            public FileMetaReturn[] Files { get; set; }
        }

        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        [Route("/v1/folder/files/{id?}")]
        public async Task<string> GetFolderContent([FromBody] FileRequest fileRequest, [FromRoute] int? id = null)
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
            User? user = db.Users.FirstOrDefault(user => user.UserID == userID);

            if (user == null)
            {
                return await new Response<string>(false, "Invalid user.").Serialize();
            }

            Folder? folder = null;

            if (id != null)
            {

                folder = db.Folders.FirstOrDefault(folder => folder.FolderID == id);

                if (folder == null)
                    return await new Response<string>(false, "Invalid folder.").Serialize();

                UserFolder? userFolder = db.UserFolders.FirstOrDefault(userFolder => userFolder.UserID == userID && userFolder.FolderID == folder.FolderID);

                if (userFolder == null)
                {
                    return await new Response<string>(false, "You do not have access to this folder.").Serialize();
                }

            }

            IQueryable<UserFile> userFiles = db.UserFiles.Where(userFile => userFile.UserID == userID);
            IQueryable<UserFolder> userFolders = db.UserFolders.Where(userFolder => userFolder.UserID == userID);

            FileData[] files = db.Files.Where(file => userFiles.FirstOrDefault(link => link.FileID == file.FileID) != null).ToArray();
            Folder[] folders = db.Folders.Where(folder => userFolders.FirstOrDefault(link => link.FolderID == folder.FolderID) != null).ToArray();

            FileMetaReturn[] fileData = new FileMetaReturn[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                fileData[i] = new FileMetaReturn()
                {
                    FileID = files[i].FileID,
                    Filename = files[i].FileName,
                    Filetype = files[i].FileType
                };
            }

            FolderDataReturn[] folderData = new FolderDataReturn[folders.Length];
            for (int i = 0; i < folderData.Length; i++)
            {
                folderData[i] = new()
                {
                    FolderID = folders[i].FolderID,
                    Foldername = folders[i].FolderName
                };
            }

            FolderContentReturn folderContentReturn = new()
            {
                Files = fileData,
                Folders = folderData
            };

            return await new Response<FolderContentReturn>(true, folderContentReturn).Serialize();

        }

    }
}
