
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StorageController.Data;
using StorageController.Data.Models;
using static StorageController.Controllers.v2.FileGetController_v2;

namespace StorageController.Controllers.v2
{
    [Controller]
    public class GetSharedFile : Controller
    {

        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        [Route("/v2/file/shared")]
        public async Task<string> GetSharedFiles()
        {

            Response<string> authResponse = await UserUtils.AuthFromHeader(Request);

            if (!authResponse.Success)
                return await authResponse.Serialize();

            int userID = int.Parse(authResponse.Message);

            DataHandler db = new();

            IEnumerable<UserFile> sharedFiles = db.UserFiles.Where(link => link.UserID == userID && link.Privilege == "Viewer"
                                                                && db.FileBin.FirstOrDefault(binFile => binFile.FileID == link.FileID) == null);
            IEnumerable<FileData> sharedFileData = db.Files.Where(link => sharedFiles.FirstOrDefault(shared => shared.FileID == link.FileID) != null);

            List<FileMetaReturn> fileData = new List<FileMetaReturn>();
            foreach (FileData sharedFile in sharedFileData)
            {

                FileMetaReturn data = new();
                data.Filetype = sharedFile.FileType;
                data.FileID = sharedFile.FileID;
                data.Filename = sharedFile.FileName;

                fileData.Add(data);
            }

            return await new Response<List<FileMetaReturn>>(true, fileData).Serialize();

        }

        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        [Route("/v2/file/sharing")]
        public async Task<string> GetSharingFiles()
        {

            Response<string> authResponse = await UserUtils.AuthFromHeader(Request);

            if (!authResponse.Success)
                return await authResponse.Serialize();

            int userID = int.Parse(authResponse.Message);

            DataHandler db = new();

            IEnumerable<UserFile> sharedFiles = db.UserFiles.Where(link => link.OwnerID == userID && link.Privilege == "Viewer"
                                                                && db.FileBin.FirstOrDefault(binFile => binFile.FileID == link.FileID) == null);
            IEnumerable<FileData> sharedFileData = db.Files.Where(link => sharedFiles.FirstOrDefault(shared => shared.FileID == link.FileID) != null);

            Dictionary<int, FileMetaReturn> fileData = new Dictionary<int, FileMetaReturn>();
            foreach (FileData sharedFile in sharedFileData)
            {

                FileMetaReturn data = new();
                data.Filetype = sharedFile.FileType;
                data.FileID = sharedFile.FileID;
                data.Filename = sharedFile.FileName;

                if (!fileData.ContainsKey(sharedFile.FileID))
                    fileData.Add(data.FileID, data);
            }

            return await new Response<List<FileMetaReturn>>(true, fileData.Values.ToList()).Serialize();

        }

        public struct DisplayUserData
        {
            public int UserID { get; set; }
            public string Username { get; set; }
        }

        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        [Route("/v2/file/sharing/users/{id}")]
        public async Task<string> GetSharingFileUsers([FromRoute] int id)
        {

            Response<string> authResponse = await UserUtils.AuthFromHeader(Request);

            if (!authResponse.Success)
                return await authResponse.Serialize();

            int userID = int.Parse(authResponse.Message);

            if (!FileUtils.DoesUserOwnFile(id, userID))
                return await new Response<string>(false, "You don't own this file").Serialize();

            DataHandler db = new();

            IEnumerable<UserFile> sharedFiles = db.UserFiles.Where(link => link.FileID == id && link.Privilege == "Viewer" && link.OwnerID == userID);

            List<int> sharedUserIDs = new List<int>();
            foreach (UserFile sharedFile in sharedFiles)
                sharedUserIDs.Add(sharedFile.UserID);

            IEnumerable<User> sharedUsers = UserUtils.GetUsersFromID(sharedUserIDs);

            List<DisplayUserData> users = new List<DisplayUserData>();
            foreach (User user in sharedUsers)
            {
                users.Add(new DisplayUserData
                {
                    UserID = user.UserID,
                    Username = user.Username
                });
            }

            return await new Response<List<DisplayUserData>>(true, users).Serialize();

        }

    }
}
