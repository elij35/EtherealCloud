
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace StorageController
{
    public class Program
    {

        public static DataHandler dataHandler;
        public static string SECURITY_KEY = "ethereal-controller-signing-key-temp";

        public static void Main(string[] args)
        {

            DataHandler dataHandler = DataHandler.instance;

            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(settings =>
            {
                settings.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateAudience = true,
                    ValidAudience = "user",
                    ValidateIssuer = true,
                    ValidIssuer = "storage-controller",
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SECURITY_KEY))
                };
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
