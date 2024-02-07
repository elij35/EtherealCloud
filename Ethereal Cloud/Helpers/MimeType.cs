using Microsoft.AspNetCore.StaticFiles;

namespace Ethereal_Cloud.Helpers
{
    public class MimeType
    {
        public static string GetMimeType(string fileName)
        {
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(fileName, out var contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }
    }
}
