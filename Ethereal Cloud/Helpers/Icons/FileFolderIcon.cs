namespace Ethereal_Cloud.Helpers.Icons
{
    public class FileFolderIcon
    {
        public static string GetIcon(string fileType)
        {
            //looks at files Mime type and gets appropriate image
            switch (fileType.ToLower())
            {
                // Folder
                case "folder":
                    return "folder.svg";

                // Images
                case "image/png":
                case "image/jpeg":
                case "image/gif":
                    return "gen-image.svg";

                // Audio
                case "audio/mpeg":
                    return "gen-music.svg";

                // Video
                case "video/mp4":
                    return "gen-video.svg";

                // PDF
                case "application/pdf":
                    return "file-pdf.svg";

                // Office products
                case "application/vnd.openxmlformats-officedocument.wordprocessingml.document":
                case "application/msword":
                    return "file-word.svg"; // Word Document (.docx)

                case "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet":
                case "application/vnd.ms-excel":
                    return "file-excel.svg"; // Excel Spreadsheet (.xlsx)

                case "application/vnd.openxmlformats-officedocument.presentationml.presentation":
                case "application/vnd.ms-powerpoint":
                    return "file-powerpoint.svg"; // PowerPoint Presentation (.pptx)

                case "application/x-mspublisher":
                    return "file-publisher.svg"; // Publisher (.pub)

                case "application/msaccess":
                    return "file-access.svg"; // Access Database (.accdb)

                // Executable
                case "application/x-msdownload":
                case "application/octet-stream":
                case "application/vnd.microsoft.portable-executable":
                    return "file-executable.svg"; // Executable (.exe)

                //JSON
                case "application/json":
                    return "file-json.svg";

                //Plain text
                case "text/plain":
                    return "file-text.svg";

                // Anything else
                default:
                    return "gen-unknown.svg"; // Unknown type

            }

        }

    }


}
