using Ethereal_Cloud.Helpers;
using Ethereal_Cloud.Models.Account;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ethereal_Cloud.Pages
{
    public class AccountModel : PageModel
    {
        [BindProperty]
        public ChangePasswordDetails changePasswordDetails { get; set; }

        public async Task OnPostChangePasswordAsync()
        {
            if (!ModelState.IsValid)
            {
                // Invalid model
                return;
            }

            //create body object
            var dataObject = new Dictionary<string, object?>
            {
                { "NewPassword", changePasswordDetails.NewPassword }
            };

            //Make request
            var response = await ApiRequestV2.Files(HttpContext, "v2/user/password", true, dataObject);

            if (response != null)
            {
                //Valid login
                ViewData["SuccessMessage"] = "Success: Password has been changed.";
            }
            else
            {
                // Invalid
                ViewData["FailureMessage"] = "Invalid: Failed to change.";
            }
        }



        public async Task OnPostLogoutAsync()
        {
            HttpContext.Session.Clear();
            Response.Redirect("/Login");
        }
    }
}
