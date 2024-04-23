using System.Net;
using System.Net.Mail;

namespace Ethereal_Cloud.Helpers
{
    public class EmailManagement
    {
        public static string Send2FAEmail(string userEmail)
        {
            string email = "EtherealCloudGroup@outlook.com";
            string pass = "Phew0=che=lZiyo&rLSwl9rimud7yo6E@r1phutRuc5lxeWri2otr2n7z+zotO3i";

            SmtpClient client = new SmtpClient("smtp-mail.outlook.com");
            client.Port = 587;
            client.EnableSsl = true;

            client.Credentials = new NetworkCredential(email, pass);

            Random random = new Random();
            int genCode = random.Next(100000, 1000000);

            MailMessage message = new MailMessage(email, userEmail);
            message.Subject = "Verification Code";
            message.IsBodyHtml = true;

            message.Body = @"<html>
            <body>
                <h1>Welcome to Ethereal Cloud</h1>
                <p>If you did not create an account with us, please ignore this email</p>
                <p>Here is your verification code: " + genCode + @"</p>

            </body>
            </html>"; 
                
            try
            {
                client.Send(message);

            }
            catch (Exception)
            {
                return null;
            }

            return $"{genCode}";
        }
    }
}
