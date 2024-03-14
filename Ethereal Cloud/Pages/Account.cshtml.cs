using Ethereal_Cloud.Helpers;
using Ethereal_Cloud.Models.Account;
using Ethereal_Cloud.Models.Login;
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
                Logger.LogToConsole(ViewData, "Invalid: Model error");
                return;
            }

            //create body object
            var dataObject = new Dictionary<string, object?>
            {
                { "NewPassword", changePasswordDetails.NewPassword }
            };

            //Make request
            var response = await ApiRequestV2.Files(ViewData, HttpContext, "v2/user/password", true, dataObject);

            if (response != null)
            {
                //Valid login
                Logger.LogToConsole(ViewData, "Successfull change of password to" + changePasswordDetails.NewPassword);
                ViewData["SuccessMessage"] = "Success: Password has been changed.";
            }
            else
            {
                Logger.LogToConsole(ViewData, "Invalid: Failed to change");
                ViewData["FailureMessage"] = "Invalid: Failed to change.";
            }
        }



        public async Task OnPostLogoutAsync()
        {
            AuthTokenManagement.RemoveToken(HttpContext);
            Response.Redirect("/Login");
        }
    }
}
