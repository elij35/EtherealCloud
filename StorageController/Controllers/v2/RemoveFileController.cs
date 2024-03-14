using Microsoft.AspNetCore.Mvc;
using StorageController.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;
using StorageController.Data.Models;

namespace StorageController.Controllers.v2
{

    [ApiController]
    public class RemoveFileController : Controller
    {

        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        [Route("/v2/file/remove/{int}")]
        public async Task<string> DeleteFileController([FromRoute] int id)
        {

            string? auth = Request.Headers.Authorization.FirstOrDefault();

            if (auth == null)
                return await new Response<string>(false, "Auth Header Required.").Serialize();

            Response<string> authResponse = await AuthManager.AuthorizeUser(auth);

            if (!authResponse.Success)
                return await authResponse.Serialize();

            int userID = int.Parse(authResponse.Message);

            DataHandler db = new();

            UserFile? fileLink = db.UserFiles.Where(fileLink => fileLink.FileID == id && fileLink.UserID == userID).FirstOrDefault();

            if (fileLink == null)
                return await new Response<string>(false, "File link not found").Serialize();

            FileData? fileData = db.Files.Where(file => file.FileID == id).FirstOrDefault();

            if (fileData == null)
                return await new Response<string>(false, "File not found").Serialize();

            FileBin removedFile = new FileBin();
            removedFile.FileID = id;

            await db.AddAsync(removedFile);
            await db.SaveChangesAsync();

            return await new Response<string>(true, "File removed successfully.").Serialize();

        }

        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        [Route("/v2/folder/remove/{int}")]
        public async Task<string> DeleteFolderController([FromRoute] int id)
        {

            string? auth = Request.Headers.Authorization.FirstOrDefault();

            if (auth == null)
                return await new Response<string>(false, "Auth Header Required.").Serialize();

            Response<string> authResponse = await AuthManager.AuthorizeUser(auth);

            if (!authResponse.Success)
                return await authResponse.Serialize();

            int userID = int.Parse(authResponse.Message);

            DataHandler db = new();

            UserFolder? folderLink = db.UserFolders.Where(folderLink => folderLink.FolderID == id && folderLink.UserID == userID).FirstOrDefault();

            if (folderLink == null)
                return await new Response<string>(false, "Folder link not found").Serialize();

            Folder? folderData = db.Folders.Where(folder => folder.FolderID == id).FirstOrDefault();

            if (folderData == null)
                return await new Response<string>(false, "Folder not found").Serialize();

            FolderBin removedFolder = new FolderBin();
            removedFolder.FolderID = id;

            await db.FolderBin.AddAsync(removedFolder);
            await db.SaveChangesAsync();

            return await new Response<string>(true, "Folder removed successfully.").Serialize();

        }

    }
}
