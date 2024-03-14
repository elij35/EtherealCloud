namespace Ethereal_Cloud.Helpers
{
    public class AuthTokenManagement
    {
        public static void SetToken(HttpContext context, string token)
        {
            var options = new CookieOptions
            {
                HttpOnly = true,
                Secure = context.Request.IsHttps, //for HTTPS
                SameSite = SameSiteMode.Lax, //cant be send with 3rd party websites
                MaxAge = TimeSpan.FromMinutes(30),
                IsEssential = true
            };
            context.Response.Cookies.Append("AuthToken", token, options);
        }

        public static string? GetToken(HttpContext context)
        {
            var cookie = context.Request.Cookies["AuthToken"];

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

        public static void RemoveToken(HttpContext context)
        {
            context.Response.Cookies.Delete("AuthToken");
        }
    }
}