
namespace StorageController
{
    public class Program
    {

        public static DataHandler dataHandler;

        public static void Main(string[] args)
        {

            string db_ip = Environment.GetEnvironmentVariable("DB_IP");
            string db_pass = Environment.GetEnvironmentVariable("DB_PASS");

            if (db_ip == null || db_pass == null )
            {
                Environment.Exit(1);
            }

            dataHandler = new DataHandler(db_ip, db_pass);

            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

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
