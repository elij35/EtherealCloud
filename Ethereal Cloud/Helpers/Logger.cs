
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Ethereal_Cloud.Helpers
{
    public static class Logger
    {
        //Viewdata is only used for console logging
        public static void LogToConsole(ViewDataDictionary viewData, string content)
        {
            viewData["logger"] = DateTime.Now.ToString("hh:mm:ss") + " : " + content;
        }

    }
}
