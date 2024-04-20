using Ethereal_Cloud.Models.Upload.Get.Folder;

namespace Ethereal_Cloud.Helpers
{
    public class SortHelper
    {
        public static List<FolderContentDisplay> SortDisplay(HttpContext context, List<FolderContentDisplay> displayList)
        {
            bool sortAlphabeticaly = SortManagement.GetSorting(context);


            if (sortAlphabeticaly)
            {
                // Alphabetical sorting
                displayList = displayList.OrderBy(item => item.Type == "Folder" ? 0 : 1)
                                                .ThenBy(item => item.Name)
                                                .ToList();
            }
            else
            {
                // Backwards alphabetical
                displayList = displayList.OrderByDescending(item => item.Type == "Folder" ? 0 : 1)
                                .ThenByDescending(item => item.Name)
                                .ToList();
            }


            return displayList;
        }

    }

}

