using Ethereal_Cloud.Pages;
using NuGet.Protocol.Plugins;

namespace Ethereal_Cloud.Helpers
{
    public class DisplayListManagement
    {
        public static void Set(HttpContext context, string key, string? value)
        {
            var options = new CookieOptions
            {
                HttpOnly = true,
                Secure = context.Request.IsHttps, //for HTTPS
                SameSite = SameSiteMode.Lax,
                IsEssential = true
            };

            context.Response.Cookies.Append(key, value, options);
        }

        public static string? Get(HttpContext context, string key)
        {
            var cookie = context.Request.Cookies[key];

            if (cookie != null)
            {
                return cookie;
            }
            else
            {
                Set(context, key, null);
            }

            return context.Request.Cookies[key];
        }

        public static void Remove(HttpContext context, string key)
        {
            context.Response.Cookies.Delete(key);
        }
    }


}
