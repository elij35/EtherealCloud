using Ethereal_Cloud.Pages;
using NuGet.Protocol.Plugins;

namespace Ethereal_Cloud.Helpers
{
    public class CookieManagement
    {
        public static void Set(HttpContext context, string key, string value)
        {
            var options = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                IsEssential = true
            };

            if (key == "AuthToken")
            {
                options.MaxAge = TimeSpan.FromMinutes(30);
            }

            context.Response.Cookies.Append(key, value, options);
        }

        public static string? Get(HttpContext context, string key)
        {
            return context.Request.Cookies[key];
        }

        public static void Remove(HttpContext context, string key)
        {
            context.Response.Cookies.Delete(key);
        }
    }


}
