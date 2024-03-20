using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using StorageController.Data;
using StorageController.Data.Models;

namespace StorageController.Controllers.v1
{

    [ApiController]
    [Route("/v1/user/signup")]
    public class SignupController : Controller
    {

        public struct SignupParams
        {
            public string UserName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }

        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<string> Index([FromBody] SignupParams signupParams)
        {

            DataHandler dataHandler = new();

            User? existingUser = dataHandler.Users.Where(user => user.Email == signupParams.Email || user.Username == signupParams.UserName).FirstOrDefault();

            if (existingUser != null)
            {
                return await new Response<string>(false, "User already exists").Serialize();
            }

            string salt = SecurityUtils.GenerateSalt();
            string hash = SecurityUtils.HashPassword(signupParams.Password, salt);

            User userData = new User();
            userData.Username = signupParams.UserName;
            userData.Email = signupParams.Email;
            userData.Password = hash;
            userData.PasswordSalt = salt;
            userData.Administrator = false;

            await dataHandler.Users.AddAsync(userData);
            await dataHandler.SaveChangesAsync();

            Response.StatusCode = 200;
            return await new Response<string>(true, "User successfully created.").Serialize();

        }

    }
}
