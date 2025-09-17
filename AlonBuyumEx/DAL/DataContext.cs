using AlonBuyumEx.Models;

using Microsoft.EntityFrameworkCore;

namespace AlonBuyumEx.DAL
{
    public class DataContext:DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
        public DbSet<DataModel> Data { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlite("Data Source=Data.db");
        }
    }
}
