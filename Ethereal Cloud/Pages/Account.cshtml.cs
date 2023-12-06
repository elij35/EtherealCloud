using Ethereal_Cloud.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ethereal_Cloud.Pages
{
    public class AccountModel : PageModel
    {

        public IActionResult OnPost()
        {

            return RedirectToPage();
        }


    }

}
