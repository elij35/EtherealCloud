using System.Security.Cryptography;
using System.Text;

namespace Bucket
{
    public class FileUtils
    {

        public static async Task<bool> WriteFile(FileStream fileStream, string fileContent)
        {

            try
            {
                fileStream.Write(Encoding.UTF8.GetBytes(fileContent));
                fileStream.Flush();
            }
            catch
            {
                return false;
            }

            return true;

        }

        public static async Task<string> RandomFileName()
        {

            byte[] nameBytes = new byte[20];
            RandomNumberGenerator.Fill(nameBytes);

            return BitConverter.ToString(nameBytes).Replace("-", "");

        }

    }
}
