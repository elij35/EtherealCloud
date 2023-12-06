using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace StorageController.Controllers
{

    [Controller]
    [Route("/user/signup")]
    public class SignupController : Controller
    {

        string[] expected_parameters =
        {
            "username",
            "email",
            "password"
        };

        [HttpPost]
        public async void Index()
        {

            Stream body_stream =  Request.Body;

            Dictionary<string, string> body_parameters = HTTPUtils.ConvertParamsToDictionary(body_stream, (int)Request.ContentLength);

            if (!HTTPUtils.CheckRequestParameters(body_parameters, expected_parameters, Response))
                return;

            string sql_addUser = "INSERT INTO ethereal.Users (Username, Email, Password, Administrator)" +
                "VALUES (@Username, @Email, @Password, 0)";

            SqlParameter[] sqlParameters = 
            {
                new SqlParameter("@Username", body_parameters["username"]),
                new SqlParameter("@Email", body_parameters["email"]),
                new SqlParameter("@Password", body_parameters["password"])
            };

            DataHandler dataHandler = DataHandler.instance;
            int rows_affected = await dataHandler.ParametizedNonQuery(sql_addUser, sqlParameters);

            if (rows_affected < 1)
            {
                Response.StatusCode = 500;
                return;
            }

            Response.StatusCode = 200;
        }
    }
}
