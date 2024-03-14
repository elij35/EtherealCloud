using Microsoft.AspNetCore.Mvc;
using StorageController.Data.Models;
using StorageController.Data;
using System.IdentityModel.Tokens.Jwt;
using Azure.Core;

namespace StorageController.Controllers.v2
{

    [ApiController]
    public class FileGetController_v2 : Controller
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

        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [Route("/v2/file/{id}")]
        public async Task<string> GetFile([FromRoute] int id)
        {

            string? auth = Request.Headers.Authorization.FirstOrDefault();

            if (auth == null || !auth.StartsWith("Bearer "))
                return await new Response<string>(false, "Authorization Header missing or in wrong format.").Serialize();

            string token = auth.Substring("Bearer ".Length).Trim();

            Response<string> authResponse = await AuthManager.AuthorizeUser(token);

            if (!authResponse.Success)
                return await authResponse.Serialize();

            int userID = int.Parse(authResponse.Message);

            DataHandler db = new();

            FileData? file = db.Files.FirstOrDefault(file => file.FileID == id 
                                                          && db.FileBin.FirstOrDefault(removedFile => removedFile.FileID == file.FileID) == null);

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
        [Route("/v2/folder/files/{id?}")]
        public async Task<string> GetFolderContent([FromRoute] int? id = null)
        {

            string? auth = Request.Headers.Authorization.FirstOrDefault();

            if (auth == null || !auth.StartsWith("Bearer "))
                return await new Response<string>(false, "Authorization Header missing or in wrong format.").Serialize();

            string token = auth.Substring("Bearer ".Length).Trim();

            Response<string> authResponse = await AuthManager.AuthorizeUser(token);

            if (!authResponse.Success)
                return await authResponse.Serialize();

            int userID = int.Parse(authResponse.Message);

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

            IQueryable<UserFile> userFiles = db.UserFiles.Where(userFile => userFile.UserID == userID
                                          && db.FileBin.FirstOrDefault(removedFile => removedFile.FileID == userFile.FileID) == null); ;
            IQueryable<UserFolder> userFolders = db.UserFolders.Where(userFolder => userFolder.UserID == userID
                                          && db.FolderBin.FirstOrDefault(removedFolder => removedFolder.FolderID == userFolder.FolderID) == null); ;

            FileData[] files = db.Files.Where(file => userFiles.FirstOrDefault(link => link.FileID == file.FileID) != null && file.FolderID == id).ToArray();
            Folder[] folders = db.Folders.Where(folder => userFolders.FirstOrDefault(link => link.FolderID == folder.FolderID) != null && folder.ParentID == id).ToArray();

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

        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        [Route("/v2/bin")]
        public async Task<string> GetBinContent()
        {

            string? auth = Request.Headers.Authorization.FirstOrDefault();

            if (auth == null || !auth.StartsWith("Bearer "))
                return await new Response<string>(false, "Authorization Header missing or in wrong format.").Serialize();

            string token = auth.Substring("Bearer ".Length).Trim();

            Response<string> authResponse = await AuthManager.AuthorizeUser(token);

            if (!authResponse.Success)
                return await authResponse.Serialize();

            int userID = int.Parse(authResponse.Message);

            DataHandler db = new();
            User? user = db.Users.FirstOrDefault(user => user.UserID == userID);

            if (user == null)
            {
                return await new Response<string>(false, "Invalid user.").Serialize();
            }

            IQueryable<UserFile> userFiles = db.UserFiles.Where(userFile => userFile.UserID == userID
                                          && db.FileBin.FirstOrDefault(removedFile => removedFile.FileID == userFile.FileID) != null);
            IQueryable<UserFolder> userFolders = db.UserFolders.Where(userFolder => userFolder.UserID == userID
                                              && db.FolderBin.FirstOrDefault(removedFolder => removedFolder.FolderID == userFolder.FolderID) != null);

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
