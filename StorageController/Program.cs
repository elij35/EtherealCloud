namespace StorageController
{
    public class Program
    {
        public static void Main(string[] args)
        {
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
