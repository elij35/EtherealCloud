using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using StorageController.Data;
using System.Data;

namespace StorageController.Controllers
{

    [ApiController]
    [Route("/user/login")]
    public class LoginController : Controller
    {

        public struct UserStruct
        {
            public string Username { get; set; }
        }

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

            // SQL for selecting the user's data
            string sql_checkUser = "SELECT * FROM ethereal.Users WHERE Username = @Username OR Email = @Email";

            // SQL parameters
            SqlParameter[] sqlParameters =
            {
                new SqlParameter("@Username", loginParams.Username),
                new SqlParameter("@Email", loginParams.Username)
            };

            // Getting the data handler and sending the SQL 
            DataHandler dataHandler = DataHandler.instance;
            DataTable table = await dataHandler.ParametizedQuery(sql_checkUser, sqlParameters);

            // Getting the data table data as a dictionary
            Dictionary<string, string[]>? entries = await dataHandler.DataTableToDictionary(table);

            // Ensuring there is a user and the passwords match
            if (entries == null || entries["Password"][0] != loginParams.Password)
            {
                return await new Response<string>(false, "Incorrect credentials.").Serialize();
            }

            UserStruct userData = new UserStruct()
            {
                Username = entries["Username"][0]
            };

            // If it reached here, all checks passed and the user exists and the password matched
            return await new Response<UserStruct>(true, userData).Serialize();
        }
    }
}
