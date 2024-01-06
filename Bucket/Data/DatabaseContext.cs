using Microsoft.EntityFrameworkCore;

namespace Bucket.Data
{
    public class DatabaseContext : DbContext
    {

        public static string CONNECTION_STRING { get; private set; }

        public DatabaseContext() {

            CONNECTION_STRING = $"Server={Program.DB_IP};" +
                $"User Id=SA;" +
                $"Password={Program.DB_PASS};" +
                $"TrustServerCertificate=true;" +
                $"Initial Catalog=bucket_{Program.BUCK_ID};";

        }

        public async void EnsureTablesCreated()
        {
            Database.EnsureCreated();

            FileData.FromSql($"SET IDENTITY_INSERT [FileData] ON;");
            await SaveChangesAsync();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(CONNECTION_STRING);
        }

        public DbSet<FileData> FileData { get; set; }

    }
}
