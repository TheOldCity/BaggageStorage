using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using BaggageStorage.DataLog.Models;
using System;
using Z.EntityFramework.Plus;

namespace BaggageStorage.DataLog
{
    public class LogDbContext : DbContext
    {
        private readonly IHostingEnvironment env;
        private readonly IConfigurationRoot config;

        public DbSet<Log> Logs { get; set; }

        public DbSet<CustomAuditEntry> AuditEntries { get; set; }
        public DbSet<AuditEntryProperty> AuditEntryProperties { get; set; }

        public LogDbContext(DbContextOptions<LogDbContext> options) : base(options)
        {

        }

        public LogDbContext(DbContextOptions<LogDbContext> options, IHostingEnvironment env) : base(options)
        {
            this.env = env;

            config = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json")
                .Build();

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            var dbType = config["AppConfigurations:DatabaseType"];

            switch (dbType)
            {
                case "mssql":
                    optionsBuilder.UseSqlServer(config.GetConnectionString("LogsMsSql")); ;
                    break;
                case "postgres":
                    optionsBuilder.UseNpgsql(config.GetConnectionString("LogsPostgres")); ;
                    break;
                case "sqlite":
                    optionsBuilder.UseSqlite(config.GetConnectionString("LogsSqLite")); ;
                    break;
                case "sqlcompact":
                    optionsBuilder.UseSqlCe(config.GetConnectionString("LogsSqlCompact"));
                    break;
                default:
                    throw new Exception("DatabaseType mast be mssql or postgres in appsettings.json");
            }

            base.OnConfiguring(optionsBuilder);
        }

    }
}
