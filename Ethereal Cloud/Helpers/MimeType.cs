using Ethereal_Cloud.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.ResponseCaching;
using Microsoft.AspNetCore.StaticFiles;
using Newtonsoft.Json;
using NuGet.Protocol.Plugins;
using System.Text;

namespace Ethereal_Cloud.Helpers
{
    public class MimeType
    {
        public static string GetMimeType(string fileName) {
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(fileName, out var contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }
    }
}
