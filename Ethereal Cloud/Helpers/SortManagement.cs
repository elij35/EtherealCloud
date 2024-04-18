namespace Ethereal_Cloud.Helpers
{
    public class SortManagement
    {
        public static void SetSorting(HttpContext context, bool sortAlpha)
        {
            var options = new CookieOptions
            {
                HttpOnly = true,
                Secure = context.Request.IsHttps, //for HTTPS
                SameSite = SameSiteMode.Lax, //cant be send with 3rd party websites
                IsEssential = true
            };
            context.Response.Cookies.Append("Sort", sortAlpha.ToString(), options);
        }

        public static bool GetSorting(HttpContext context)
        {
            var cookie = context.Request.Cookies["Sort"];

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

        public static void RemoveSorting(HttpContext context)
        {
            context.Response.Cookies.Delete("Sort");
        }
    }
}