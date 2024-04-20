using System.Net.Mail;
using System.Net;

namespace Ethereal_Cloud.Helpers
{
	public class Email
	{
		public static int sending()
		{
			string email = "EtherealCloudTesting@outlook.com";
			string pass = "=pew9V_s";
			string userEmail = "rileycoulstock@gmail.com"; // Figure out receive of email and code placement

			SmtpClient client = new SmtpClient("smtp-mail.outlook.com");
			client.Port = 587;
			client.EnableSsl = true;

			client.Credentials = new NetworkCredential(email, pass);
			Random random = new Random();
			int genCode = random.Next(100000, 1000000);
			MailMessage message = new MailMessage(email, userEmail);
			message.Subject = "Verification Code";
			message.Body = $"{genCode}";
			try
			{
				client.Send(message);

			}
			catch (Exception)
			{
				throw;
			}
			
			return genCode;
		}
	}
}
