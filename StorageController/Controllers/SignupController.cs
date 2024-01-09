using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using StorageController.Data;

namespace StorageController.Controllers
{

    [ApiController]
    [Route("/user/signup")]
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

            string sql_addUser = "INSERT INTO ethereal.Users (Username, Email, Password, Administrator)" +
                "VALUES (@Username, @Email, @Password, 0)";

            SqlParameter[] sqlParameters = 
            {
                new SqlParameter("@Username", signupParams.UserName),
                new SqlParameter("@Email", signupParams.Email),
                new SqlParameter("@Password", signupParams.Password)
            };

            DataHandler dataHandler = DataHandler.instance;
            int rows_affected = await dataHandler.ParametizedNonQuery(sql_addUser, sqlParameters);

            if (rows_affected < 1)
            {
                Response.StatusCode = 500;
                return await new Response<string>(false, "An unexpected error occured.").Serialize();
            }

            Response.StatusCode = 200;
            return await new Response<string>(true, "User successfully created.").Serialize();
        }
    }
}
