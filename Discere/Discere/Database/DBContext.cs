using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Discere.Database
{
    public class DBContext : DbContext
    {
        public static string ConnectionString = "Data Source=databse.db";

        public DbSet<Card.Model> Card { get; set; }

        public DBContext() { }
        public DBContext(DbContextOptions<DBContext> options)
            : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var dbPath = Preferences.Get("DatabasePath", null);

                if (string.IsNullOrEmpty(dbPath))
                    throw new InvalidOperationException("Database not configured.");

                optionsBuilder.UseSqlite($"Data Source={dbPath}");
            }
        }
    }
}
