using Microsoft.AspNetCore.Mvc;
using StorageController.Data;
using StorageController.Data.Models;

namespace StorageController.Controllers.v2
{

    [ApiController]
    public class ChangePasswordController : Controller
    {

        public struct PasswordData
        {
            public string NewPassword { get; set; }
        }

        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        [Route("/v2/user/password")]
        public async Task<string> UserPasswordController([FromBody] PasswordData data)
        {

            string? auth = Request.Headers.Authorization.FirstOrDefault();

            if (auth == null || !auth.StartsWith("Bearer "))
                return await new Response<string>(false, "Authorization Header missing or in wrong format.").Serialize();

            string token = auth.Substring("Bearer ".Length).Trim();

            Response<string> authResponse = await AuthManager.AuthorizeUser(token);

            if (!authResponse.Success)
                return await authResponse.Serialize();

            int userID = int.Parse(authResponse.Message);

            string salt = SecurityUtils.GenerateSalt();
            string hash = SecurityUtils.HashPassword(data.NewPassword, salt);

            DataHandler db = new();

            User? user = db.Users.Where(user => user.UserID == userID).FirstOrDefault();

            if (user == null)
                return await new Response<string>(false, "No user found.").Serialize();

            user.Password = hash;
            user.PasswordSalt = salt;
            await db.SaveChangesAsync();

            return await new Response<string>(true, "Password changed successfully.").Serialize();

        }

    }
}
