using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using StorageController.Data;
using StorageController.Data.Models;
using System.Data;
using System.IdentityModel.Tokens.Jwt;

namespace StorageController.Controllers.v1
{

    [ApiController]
    [Route("/v1/user/login")]
    public class LoginController : Controller
    {

        public struct LoginParams
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<string> Index([FromBody] LoginParams loginParams)
        {

            // Getting the data handler and sending the SQL 
            DataHandler dataHandler = new();
            User? user = dataHandler.Users.Where(user => user.Username == loginParams.Username || user.Email == loginParams.Username).FirstOrDefault();

            if (user == null)
            {
                return await new Response<string>(false, "Incorrect credentials.").Serialize();
            }

            // Ensuring there is a user and the passwords match
            if (!SecurityUtils.CheckPassword(loginParams.Password, user.PasswordSalt, user.Password))
            {
                return await new Response<string>(false, "Incorrect credentials.").Serialize();
            }

            string token = await AuthManager.AuthorizeUser(user.UserID);

            // If it reached here, all checks passed and the user exists and the password matched
            return await new Response<string>(true, token).Serialize();
        }
    }
}
