using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using StorageController.Data.Models;
using System.Data;

namespace StorageController.Data
{
    public class DataHandler : DbContext
    {

        private static string ConnectionString = 
                "User id=SA;" +
                "TrustServerCertificate=True;" +
                "Initial catalog=ethereal_storage;";

        public DataHandler() { }

        public DataHandler(string DB_IP, string DB_PASS)
        {

            ConnectionString +=
                $"Data Source={DB_IP};" +
                $"Password={DB_PASS};";

            EnsureDatabaseExists();

        }

        public async void EnsureDatabaseExists()
        {

            try
            {
                Database.EnsureCreated();

                Bucket bucket = new Bucket();
                bucket.BucketIP = Environment.GetEnvironmentVariable("BUCK_IP");
                bucket.BucketPort = Environment.GetEnvironmentVariable("BUCK_PORT");

                await Buckets.AddAsync(bucket);
                await SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e}");
                return;
            }

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("ethereal");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnectionString, sqlOptions => sqlOptions.EnableRetryOnFailure(10));
        }

        public DbSet<User> Users { get; set; }
        public DbSet<FileData> Files { get; set; }
        public DbSet<UserFile> UserFiles { get; set; }
        public DbSet<Bucket> Buckets { get; set; }
        public DbSet<Folder> Folders { get; set; }
        public DbSet<UserFolder> UserFolders { get; set; }
    }
}