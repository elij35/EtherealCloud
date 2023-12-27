using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace StorageController.Controllers
{

    [ApiController]
    [Route("/api/auth")]
    public class AuthController : Controller
    {

        [HttpPost]
        public string Index()
        {

            Stream body_stream = Request.Body;

            Dictionary<string, string> form_params = HTTPUtils.ConvertParamsToDictionary(body_stream, (int)Request.ContentLength);

            return $"Authorizing user: {form_params["email"]} with password {form_params["password"]}";
        }
    }
}
