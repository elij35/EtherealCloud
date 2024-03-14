using Microsoft.AspNetCore.Mvc;
using StorageController.Data;
using StorageController.Data.Models;

namespace StorageController.Controllers.v2
{
    public class RestoreFileController : Controller
    {

        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [Route("/v2/file/restore/{id}")]
        public async Task<string> RestoreFile([FromRoute] int id)
        {

            string? auth = Request.Headers.Authorization.FirstOrDefault();

            if (auth == null)
                return await new Response<string>(false, "Auth Header Required.").Serialize();

            Response<string> authResponse = await AuthManager.AuthorizeUser(auth);

            if (!authResponse.Success)
                return await authResponse.Serialize();

            int userID = int.Parse(authResponse.Message);

            DataHandler db = new();

            UserFile? fileLink = db.UserFiles.Where(link => link.FileID == id && link.UserID == userID).FirstOrDefault();

            if (fileLink == null)
                return await new Response<string>(false, "You do not have access to this file.").Serialize();

            FileBin? removedFile = db.FileBin.Where(removed => removed.FileID == id).FirstOrDefault();

            if (removedFile == null)
                await new Response<string>(false, "File was not in the bin.").Serialize();

            db.FileBin.Remove(removedFile);
            await db.SaveChangesAsync();

            return await new Response<string>(true, "File restored.").Serialize();

        }

        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [Route("/v2/folder/restore/{id}")]
        public async Task<string> RestoreFolder([FromRoute] int id)
        {

            string? auth = Request.Headers.Authorization.FirstOrDefault();

            if (auth == null)
                return await new Response<string>(false, "Auth Header Required.").Serialize();

            Response<string> authResponse = await AuthManager.AuthorizeUser(auth);

            if (!authResponse.Success)
                return await authResponse.Serialize();

            int userID = int.Parse(authResponse.Message);

            DataHandler db = new();

            UserFolder? folderLink = db.UserFolders.Where(link => link.FolderID == id && link.UserID == userID).FirstOrDefault();

            if (folderLink == null)
                return await new Response<string>(false, "You do not have access to this folder.").Serialize();

            FolderBin? removedFolder = db.FolderBin.Where(removed => removed.FolderID == id).FirstOrDefault();

            if (removedFolder == null)
                await new Response<string>(false, "Folder was not in the bin.").Serialize();

            db.FolderBin.Remove(removedFolder);
            await db.SaveChangesAsync();

            return await new Response<string>(true, "Folder restored.").Serialize();

        }

    }
}
