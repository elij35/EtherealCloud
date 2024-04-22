using Ethereal_Cloud.Models.Upload.Get;
using System.Text.Json;

namespace Ethereal_Cloud.Helpers
{
    public class CookieManagement
    {
        public static void SetCookie(HttpContext context, string name, string token)
        {
            context.Session.SetString(name, token);
        }

        public static void RemoveCookie(HttpContext context, string name)
        {
            context.Session.Remove(name);
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
                context.Session.Clear();
                context.Response.Redirect("/Index");
                return null;
            }
        }

        public static bool GetSorting(HttpContext context)
        {
            var cookie = context.Session.GetString("Sort");

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

        public static bool GetActiveShare(HttpContext context)
        {
            var cookie = context.Session.GetString("Shared");

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

        public static List<FolderDataRecieve> GetFolderPath(HttpContext context)
        {
            var cookie = context.Session.GetString("FolderPath");

            if (cookie != null)
            {
                return JsonSerializer.Deserialize<List<FolderDataRecieve>>(cookie);
            }
            else
            {
                return new List<FolderDataRecieve>();
            }

        }

    }
}