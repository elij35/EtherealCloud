using Ethereal_Cloud.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using NuGet.Protocol.Plugins;
using System.Text.Json;

namespace Ethereal_Cloud.Helpers
{
    public class PathManagement
    {
        public static void Set(HttpContext context, string value)
        {
            var options = new CookieOptions
            {
                HttpOnly = true,
                Secure = context.Request.IsHttps, //for HTTPS
                SameSite = SameSiteMode.Lax,
                IsEssential = true
            };

            context.Response.Cookies.Append("FolderPath", value, options);
        }

        public static List<FolderDataRecieve> Get(HttpContext context)
        {
            var cookie = context.Request.Cookies["FolderPath"];

            if (cookie != null)
            {
                return JsonSerializer.Deserialize<List<FolderDataRecieve>>(cookie);
            }
            else
            {
                return new List<FolderDataRecieve>();
            }

        }

        public static void Remove(HttpContext context)
        {
            context.Response.Cookies.Delete("FolderPath");
        }

        public static int? GetCurrentFolderId(HttpContext context)
        {
            var folderList = Get(context);

            if (folderList != null && folderList.Any())
            {
                var lastFolderData = folderList.Last();

                return lastFolderData.FolderID;
            }
            else
            {
                //empty or null list
                return null;
            }

        }

        public static bool GoBackInFolderPath(HttpContext context, int folderId, ViewDataDictionary viewData)
        {
            //true is success, false is failure

            Logger.LogToConsole(viewData, "Made it this far");


            

            //-2 means goto root
            if (folderId == -2)
            {
                //folderId is null so set the folderpath back to the root
                Set(context, "");
                return true;
            }



            var folderList = Get(context);

            if (folderList == null || folderList.Count < 1)
            {
                //empty or null list
                return false;
            }


            //-1 means goback 1 folder
            if (folderId == -1)
            {
                //Remove the last element
                folderList.RemoveAt(folderList.Count - 1);

                Set(context, JsonSerializer.Serialize(folderList));

                return true;
            }


            //find the folder with a matching folderId
            int indexToRemove = folderList.FindIndex(folder => folder.FolderID == folderId);

            if (indexToRemove != -1)
            {
                //remove the elements after
                folderList = folderList.Take(indexToRemove + 1).ToList();

                Set(context, JsonSerializer.Serialize(folderList));

                return true;
            }
            else
            {
                //folderId wasnt found
                return false;
            }


        }
    }


}
