using StorageController.Data;
using StorageController.Data.Models;

namespace StorageController
{
    public class FileUtils
    {

        public static bool DoesUserOwnFile(int fileID, int userID)
        {

            DataHandler db = new DataHandler();
            UserFile? fileLink = db.UserFiles.Where(link => link.FileID == fileID && link.UserID == userID && link.Privilege == "Owner").FirstOrDefault();

            return fileLink != null;

        }

        public static bool IsFileSharedWithUser(int fileID, int userID)
        {

            DataHandler db = new DataHandler();
            UserFile? fileLink = db.UserFiles.Where(link => link.FileID == fileID && link.UserID == userID && link.Privilege == "Viewer").FirstOrDefault();

            return fileLink != null;

        }

        public static bool CanUserAccessFile(int fileID, int userID)
        {

            DataHandler db = new DataHandler();
            UserFile? fileLink = db.UserFiles.Where(link => link.FileID == fileID && link.UserID == userID).FirstOrDefault();

            return fileLink != null;

        }

    }
}
