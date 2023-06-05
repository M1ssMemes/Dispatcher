using Dispatcher.DbModels;
using Microsoft.EntityFrameworkCore;

namespace Dispatcher
{
    public class MyDbContext : DbContext
    {
        public DbSet<ClientModel> Clients { get; set; }
        public DbSet<JournalModel> Journals { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=C:\\Users\\memes\\Desktop\\homework\\rgu\\Diplom\\TestFolders\\db\\Dispatcher.sqlite");
            base.OnConfiguring(optionsBuilder);
        }
    }
}
