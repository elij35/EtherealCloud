using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Text.Json;

namespace Ethereal_Cloud.Helpers
{
    public class PathManagement
    {
        public static int? GetCurrentFolderId(HttpContext context)
        {
            var folderList = CookieManagement.GetFolderPath(context);

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
                CookieManagement.RemoveCookie(context, "FolderPath");
                return true;
            }



            var folderList = CookieManagement.GetFolderPath(context);

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

                CookieManagement.SetCookie(context, "FolderPath", JsonSerializer.Serialize(folderList));

                return true;
            }


            //find the folder with a matching folderId
            int indexToRemove = folderList.FindIndex(folder => folder.FolderID == folderId);

            if (indexToRemove != -1)
            {
                //remove the elements after
                folderList = folderList.Take(indexToRemove + 1).ToList();

                CookieManagement.SetCookie(context, "FolderPath", JsonSerializer.Serialize(folderList));

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
