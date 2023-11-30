using Microsoft.AspNetCore.Mvc;

namespace StorageController.Controllers
{

    [ApiController]
    [Route("/api/auth")]
    public class AuthController : Controller
    {

        [HttpGet]
        public string Index()
        {
            return "test authorization endpoint";
        }
    }
}
