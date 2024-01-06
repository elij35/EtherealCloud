using Bucket.Data;

namespace Bucket
{
    public class Program
    {

        public static string DB_IP;
        public static string DB_PASS;
        public static string BUCK_ID;

        public static void Main(string[] args)
        {

            DB_IP = Environment.GetEnvironmentVariable("DB_IP");
            DB_PASS = Environment.GetEnvironmentVariable("DB_PASS");
            BUCK_ID = Environment.GetEnvironmentVariable("BUCK_ID");

            if (DB_IP == null || DB_PASS == null || BUCK_ID == null) { Environment.Exit(1); }


            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run("http://[::]:8070");
        }
    }
}
