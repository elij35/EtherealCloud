
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
        [Route("/v2/file/sharing")]
        public async Task<string> GetSharedFiles()
        {

            Response<string> authResponse = await UserUtils.AuthFromHeader(Request);

            if (!authResponse.Success)
                return await authResponse.Serialize();

            int userID = int.Parse(authResponse.Message);

            DataHandler db = new();

            IEnumerable<UserFile> sharedFiles = db.UserFiles.Where(link => link.UserID == userID && link.Privilege == "Viewer");

            List<FileMetaReturn> fileData = new List<FileMetaReturn>();
            foreach (UserFile fileLink in sharedFiles)
            {

                FileData file = fileLink.File;

                FileMetaReturn data = new();
                data.Filetype = file.FileType;
                data.FileID = file.FileID;
                data.Filename = file.FileName;

                fileData.Add(data);
            }

            return await new Response<List<FileMetaReturn>>(true, fileData).Serialize();

        }

    }
}
