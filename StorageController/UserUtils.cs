using StorageController.Data;
using StorageController.Data.Models;

namespace StorageController
{
    public class UserUtils
    {

        public static bool DoesUsernameExist(string username)
        {

            DataHandler db = new DataHandler();
            User? user = db.Users.Where(dbUser => dbUser.Username == username).FirstOrDefault();

            return user != null;

        }

        public static IEnumerable<User> GetUsersFromID(List<int> userIDs)
        {

            DataHandler db = new();

            IEnumerable<User> users = db.Users.Where(user => userIDs.Contains(user.UserID));

            return users;

        }

        public static User? GetUserFromUsername(string username)
        {

            DataHandler db = new DataHandler();
            User? user = db.Users.Where(dbUser => dbUser.Username == username).FirstOrDefault();

            return user;

        }

        public async static Task<Response<string>> AuthFromHeader(HttpRequest Request)
        {

            string? auth = Request.Headers.Authorization.FirstOrDefault();

            if (auth == null || !auth.StartsWith("Bearer "))
                return new Response<string>(false, "Authorization Header missing or in wrong format.");

            string token = auth.Substring("Bearer ".Length).Trim();

            return await AuthManager.AuthorizeUser(token);

        }

    }
}
