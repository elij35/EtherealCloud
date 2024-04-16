using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StorageController.Data;
using StorageController.Data.Models;

namespace StorageController.Controllers.v2
{
    [Controller]
    public class FileShareController : Controller
    {

        public struct ShareStruct
        {
            public string ShareUsername { get; set; }
        }

        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        [Route("/v2/file/share/{id}")]
        public async Task<string> ShareFile([FromRoute] int id, [FromBody] ShareStruct shareInfo)
        {

            Response<string> authResponse = await UserUtils.AuthFromHeader(Request);

            if (!authResponse.Success)
                return await authResponse.Serialize();

            int userID = int.Parse(authResponse.Message);

            if (!FileUtils.DoesUserOwnFile(id, userID))
                return await new Response<string>(false, "You don't own this file.").Serialize();

            if (!UserUtils.DoesUsernameExist(shareInfo.ShareUsername))
                return await new Response<string>(false, "Username does not exist.").Serialize();

            User? target = UserUtils.GetUserFromUsername(shareInfo.ShareUsername);
            if (target == null)
                return await new Response<string>(false, "Could not get user.").Serialize();

            DataHandler db = new();

            if (FileUtils.IsFileSharedWithUser(id, target.UserID))
            {

                await db.UserFiles.Where(link => link.UserID == target.UserID && link.FileID == id).ExecuteDeleteAsync();
                await db.SaveChangesAsync();

                return await new Response<string>(true, "File successfully unshared.").Serialize();

            }

            UserFile shareLink = new();
            shareLink.FileID = id;
            shareLink.UserID = userID;
            shareLink.Privilege = "Viewer";

            await db.UserFiles.AddAsync(shareLink);
            await db.SaveChangesAsync();

            return await new Response<string>(true, "File successfully shared.").Serialize();

        }


    }
}
