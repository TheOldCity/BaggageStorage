using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore.Design;
using BaggageStorage.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore;
using BaggageStorage.DataLog;
using BaggageStorage.Services.WebSocket;

namespace BaggageStorage
{
    public class Program
    {
        private static IConfiguration config;

        // этот класс нужен для того, чтобы в консоли диспетчера пакетов можно было миграции создавать
        public class DbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
        {
            public AppDbContext CreateDbContext(string[] args)
            {
                var app = BuildWebHostDuringGen(args);
                var scope = app.Services.GetService<IServiceScopeFactory>().CreateScope();

                return scope.ServiceProvider.GetRequiredService<AppDbContext>();
            }
        }

        // этот класс нужен для того, чтобы в консоли диспетчера пакетов можно было миграции создавать
        public class LogDbContextFactory : IDesignTimeDbContextFactory<LogDbContext>
        {
            public LogDbContext CreateDbContext(string[] args)
            {
                var app = BuildWebHostDuringGen(args);
                var scope = app.Services.GetService<IServiceScopeFactory>().CreateScope();

                return scope.ServiceProvider.GetRequiredService<LogDbContext>();
            }
        }

        public static void Main(string[] args)
        {
            WebSocketMiddleware.WebSocketsList = new Dictionary<String, WebSocketMiddleware.WebSocketConnection>();

            config = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("hosting.json", optional: true)
                        .AddJsonFile("appsettings.json", optional: false)
                        .Build();

            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseConfiguration(config)
                .Build();

        public static IWebHost BuildWebHostDuringGen(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>().Build();
        }
       
    }
}
