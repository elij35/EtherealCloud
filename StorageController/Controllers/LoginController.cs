using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace StorageController.Controllers
{

    [ApiController]
    [Route("/user/login")]
    public class LoginController : Controller
    {

        string[] expected_parameters =
        {
            "username",
            "password"
        };

        [HttpPost]
        public async Task<bool> Index()
        {

            Stream body_stream = Request.Body;

            // Converting the request body to a dictionary of parameters
            Dictionary<string, string> body_parameters = HTTPUtils.ConvertParamsToDictionary(body_stream, (int)Request.ContentLength);

            // Ensuring all the required parameters are present
            if (!HTTPUtils.CheckRequestParameters(body_parameters, expected_parameters, Response))
                return false;

            // SQL for selecting the user's data
            string sql_checkUser = "SELECT * FROM ethereal.Users WHERE Username = @Username OR Email = @Email";

            // SQL parameters
            SqlParameter[] sqlParameters =
            {
                new SqlParameter("@Username", body_parameters["username"]),
                new SqlParameter("@Email", body_parameters["username"])
            };

            // Getting the data handler and sending the SQL 
            DataHandler dataHandler = DataHandler.instance;
            DataTable table = await dataHandler.ParametizedQuery(sql_checkUser, sqlParameters);

            // Getting the data table data as a dictionary
            Dictionary<string, string[]>? entries = await dataHandler.DataTableToDictionary(table);

            // Ensuring there is a user and the passwords match
            if (entries == null || entries["Password"][0] != body_parameters["password"])
            {
                return false;
            }

            // If it reached here, all checks passed and the user exists and the password matched
            return true;
        }
    }
}
