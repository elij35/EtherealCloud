using System.Security.Cryptography;
using System.Text;

namespace StorageController.Data
{
    public class SecurityUtils
    {

        public static string GenerateSalt(int saltLength = 15)
        {

            byte[] bytes = new byte[saltLength];
            RandomNumberGenerator.Fill(bytes);

            string randomString = Encoding.UTF8.GetString(bytes);

            return randomString.Replace("-", "");

        }

        public static string HashPassword(string password, string salt)
        {

            string saltedPassword = password + salt;

            byte[] passwordBytes = Encoding.UTF8.GetBytes(saltedPassword);

            SHA256Managed sha = new(); 
            
            byte[] hashedBytes = sha.ComputeHash(passwordBytes);

            StringBuilder builder = new StringBuilder();

            foreach( byte hex in hashedBytes)
                builder.AppendFormat("{0:x2}", hex);

            return builder.ToString();

        }

        public static bool CheckPassword(string password, string salt, string hash)
        {

            string passwordHash = HashPassword(password, salt);

            return passwordHash == hash;

        }

    }
}
