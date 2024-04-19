namespace Ethereal_Cloud.Helpers
{
    public class ShareManagement
    {
        public static void SetShareTab(HttpContext context, bool shared)
        {
            var options = new CookieOptions
            {
                HttpOnly = true,
                Secure = context.Request.IsHttps, //for HTTPS
                SameSite = SameSiteMode.Lax, //cant be send with 3rd party websites
                IsEssential = true
            };
            context.Response.Cookies.Append("Shared", shared.ToString(), options);
        }

        public static bool GetActiveShare(HttpContext context)
        {
            var cookie = context.Request.Cookies["Shared"];

            if (cookie != null)
            {
                if (bool.TryParse(cookie, out bool sortingValue))
                {
                    return sortingValue;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }

        public static void RemoveShare(HttpContext context)
        {
            context.Response.Cookies.Delete("Shared");
        }
    }
}