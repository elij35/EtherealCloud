using Microsoft.EntityFrameworkCore;

namespace Ethereal_Cloud.Models
{
    public class TestingDbContext : DbContext
    {
        //Think of this like sql database
        public DbSet<Account> Account {  get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("myDb");
        }


    }
}
