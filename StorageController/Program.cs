
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using StorageController.Data;
using System.Text;

namespace StorageController
{
    public class Program
    {

        public static DataHandler dataHandler;
        public static string SECURITY_KEY = "ethereal-controller-signing-key-temp";
        public static TokenValidationParameters Validation_Parameters;

        public static void Main(string[] args)
        {

            string DB_IP = Environment.GetEnvironmentVariable("DB_IP");
            string DB_PASS = Environment.GetEnvironmentVariable("DB_PASS");

            Validation_Parameters = new TokenValidationParameters()
            {
                ValidateAudience = true,
                ValidAudience = "user",
                ValidateIssuer = true,
                ValidIssuer = "storage-controller",
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SECURITY_KEY))
            };

            if (DB_IP == null || DB_PASS == null)
                Environment.Exit(-1);

            DataHandler dataHandler = new DataHandler(DB_IP, DB_PASS);

            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(settings =>
            {
                settings.TokenValidationParameters = Validation_Parameters;
            });

            builder.Services.AddControllers();

            WebApplication app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseAuthorization();
            app.MapControllers();

            // Setting the URL and port
            app.Urls.Add("http://[::]:8090");

            app.Run();

        }
    }
}
