
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
                                                                       && db.FileBin.FirstOrDefault(bin => bin.FileID == link.FileID) == null);
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


        public struct SharingFileMetaReturn
        {
            public int FileID { get; set; }
            public string Filename { get; set; }
            public string Filetype { get; set; }
            public List<SharingUsers> SharingUsers { get; set; }
        }


        public struct SharingUsers
        {
            public int UserID { get; set; }
            public string Username { get; set; }

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
                                                                    && db.FileBin.FirstOrDefault(bin => bin.FileID == link.FileID) == null);
            IEnumerable<FileData> sharedFileData = db.Files.Where(link => sharedFiles.FirstOrDefault(shared => shared.FileID == link.FileID) != null);

            Dictionary<int, SharingFileMetaReturn> fileData = new Dictionary<int, SharingFileMetaReturn>();
            foreach (FileData sharedFile in sharedFileData)
            {

                IEnumerable<UserFile> SharingFileUsers = db.UserFiles.Where(link => link.FileID == sharedFile.FileID && link.Privilege == "Viewer" && link.OwnerID == userID);

                List<int> sharedUserIDs = new List<int>();
                foreach (UserFile fileShared in SharingFileUsers)
                    sharedUserIDs.Add(fileShared.UserID);

                IEnumerable<User> sharedUsers = UserUtils.GetUsersFromID(sharedUserIDs);

                List<SharingUsers> users = new List<SharingUsers>();
                foreach (User user in sharedUsers)
                {
                    users.Add(new SharingUsers
                    {
                        UserID = user.UserID,
                        Username = user.Username
                    });
                }

                SharingFileMetaReturn data = new();
                data.Filetype = sharedFile.FileType;
                data.FileID = sharedFile.FileID;
                data.Filename = sharedFile.FileName;
                data.SharingUsers = users;

                if (!fileData.ContainsKey(sharedFile.FileID))
                    fileData.Add(data.FileID, data);
            }

            return await new Response<List<SharingFileMetaReturn>>(true, fileData.Values.ToList()).Serialize();

        }


    }
}
