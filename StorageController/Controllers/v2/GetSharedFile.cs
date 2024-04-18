
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
        [Route("/v2/file/share")]
        public async Task<string> GetSharedFiles()
        {

            Response<string> authResponse = await UserUtils.AuthFromHeader(Request);

            if (!authResponse.Success)
                return await authResponse.Serialize();

            int userID = int.Parse(authResponse.Message);

            DataHandler db = new();

            IQueryable<UserFile> sharedFiles = db.UserFiles.Where(link => link.UserID == userID && link.Privilege != "Owner");
            FileData[] files = await db.Files.Where(file => sharedFiles.Where(link => link.FileID == file.FileID) != null).ToArrayAsync();

            FileMetaReturn[] fileData = new FileMetaReturn[files.Length];
            for (int i = 0; i < files.Length; i++)
            {

                FileMetaReturn data = new();
                data.Filetype = files[i].FileType;
                data.FileID = files[i].FileID;
                data.Filename = files[i].FileName;

                fileData[i] = data;

            }

            return await new Response<FileMetaReturn[]>(true, fileData).Serialize();

        }

    }
}
