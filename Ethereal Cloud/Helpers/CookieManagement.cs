namespace Ethereal_Cloud.Helpers
{
    public class CookieManagement
    {
        public static void SetCookie(HttpContext context, string name, string token)
        {
            context.Session.SetString(name, token);
        }

        public static string? GetAuthToken(HttpContext context)
        {
            var cookie = context.Session.GetString("AuthToken");

            if (cookie != null)
            {
                return cookie;
            }
            else
            {
                context.Response.Redirect("/Index");
                return null;
            }
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

    }
}