using Microsoft.AspNetCore.Mvc;
using Ethereal_Cloud.Models;

namespace Ethereal_Cloud.LoginController
{
    public class LoginController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        // POST: /YourController/HandlePost
        [HttpPost]
        public IActionResult LoginPost(LoginModel login)
        {
            // Handle the posted data here, for example, save it to a database.

            // Redirect to a different action or view
            return RedirectToAction("Index", "Home");
        }
    }

}
