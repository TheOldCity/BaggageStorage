using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using BaggageStorage.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using BaggageStorage.Data.Models;
using Microsoft.AspNetCore.ResponseCompression;
using BaggageStorage.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Options;
using Z.EntityFramework.Plus;
using BaggageStorage.DataLog.Models;
using BaggageStorage.DataLog;
using BaggageStorage.Classes;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using BaggageStorage.Services.WebSocket;
using BaggageStorage.Services.WebSocket;

namespace BaggageStorage
{
    public class Startup
    {
        
        public static IServiceProvider ServiceProvider;        

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            #region Localization 
            //Adding Localisation to an ASP.NET Core application
            //https://andrewlock.net/adding-localisation-to-an-asp-net-core-application/
            //Globalization and localization
            //https://docs.microsoft.com/en-us/aspnet/core/fundamentals/localization
            services.AddLocalization(opts => { opts.ResourcesPath = "Resources"; });

            services.AddMvc()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix, opts => { opts.ResourcesPath = "Resources"; })
                .AddDataAnnotationsLocalization();

            services.Configure<RequestLocalizationOptions>(opts =>
            {
                //https://msdn.microsoft.com/en-us/library/cc233982.aspx
                var supportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("ru"),
                    new CultureInfo("ro"),
                    new CultureInfo("en")
                };

                opts.DefaultRequestCulture = new RequestCulture("ru");
                // Formatting numbers, dates, etc.
                opts.SupportedCultures = supportedCultures;
                // UI strings that we have localized.
                opts.SupportedUICultures = supportedCultures;

                //opts.RequestCultureProviders.Clear();
                //opts.RequestCultureProviders.Add(new CookieRequestCultureProvider());
            });
            #endregion

            // добавл€ем поддержку сессий
            services.AddSession(options =>
            {
                //options.IdleTimeout = TimeSpan.FromDays(10);                
                //options.Cookie.HttpOnly = true;
            });

            // добавл€ем контекст данных и контекст Ћогов
            services.AddDbContext<AppDbContext>(ServiceLifetime.Transient);
            services.AddDbContext<LogDbContext>(ServiceLifetime.Transient);

            // подключаем Identity Framework
            services.AddIdentity<ApplicationUser, ApplicationRole>(s =>
            {
                s.SignIn.RequireConfirmedEmail = true;
                s.Password.RequireDigit = false;
                s.Password.RequiredLength = 4;
                s.Password.RequireNonAlphanumeric = false;
                s.Password.RequireUppercase = false;
                s.Password.RequireLowercase = false;                
            })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            // добавл€ем нашу фабрику ClaimsPrincipal
            services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, AppClaimsPrincipalFactory>();

            // добавл€ем компрессию
            services.Configure<GzipCompressionProviderOptions>(options => options.Level = System.IO.Compression.CompressionLevel.Optimal);
            services.AddResponseCompression();

            services.ConfigureApplicationCookie(options =>
            {
                // ≈сть SecurityStamp в котором есть периодическа€ проверка Claims и если врем€ вышло (например, когда ничего не делаешь или когда сервер остановлен), то происходит
                // вызов AppClaimsPrincipalFactory CreateAsync, что нам не нужно т.к. сесси€ еще не создана и в ошибку вылетало
                // поэтому стандартную проверку отключаем
                options.Events = new Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationEvents();
                
                //Cookie settings, only this changes expiration
                //options.Cookie.HttpOnly = true;
                //options.Cookie.Expiration = TimeSpan.FromDays(365 * 10);
                //options.ExpireTimeSpan = TimeSpan.FromDays(365 * 10);
            });

            services.AddMvc(config =>
            {
                config.ModelBinderProviders.Insert(0, new InvariantDecimalModelBinderProvider());
            })
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver());

            // добавл€ем сервис с помощью которого можно получить информацию о HttpContext из любого места через DiJ
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IActionContextAccessor, ActionContextAccessor>(); 
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IServiceProvider serviceProvider,
            IApplicationLifetime applicationLifetime)
        {
            loggerFactory.AddFile("Logs/webapp-{Date}.txt");

            ServiceProvider = serviceProvider;
            var _logger = loggerFactory.CreateLogger("Startup");

            // локализаци€
            var options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(options.Value);

            app.UseResponseCompression();

            // јвтоматическа€ миграци€
            try
            {
                using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
                {
                    // основной контекст данных
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    if (!dbContext.AllMigrationsApplied())
                    {
                        dbContext.Database.Migrate();
                        dbContext.EnsureSeedData(scope.ServiceProvider).Wait();
                    }

                    // контекст данных дл€ логировани€
                    var dbLogContext = scope.ServiceProvider.GetRequiredService<LogDbContext>();

                    if (!dbLogContext.AllMigrationsApplied())
                    {
                        dbLogContext.Database.Migrate();
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.LogCritical(Utils.GetFullError(ex));
            }

            // Uncomment to use pre-17.2 routing for .Mvc() and .WebApi() data sources
            // DevExtreme.AspNet.Mvc.Compatibility.DataSource.UseLegacyRouting = true;
            // Uncomment to use pre-17.2 behavior for the "required" validation check
            // DevExtreme.AspNet.Mvc.Compatibility.Validation.IgnoreRequiredForBoolean = false;

            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseSession();

            
            // ƒобавл€ем наш Websocket Middleware
            app.UseRtiWebSocket();

            

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            #region Audit

            // EF + Audit easily tracks changes, exclude / include entity or property and auto save audit entries in the database.
            // http://entityframework-plus.net/audit
            AuditManager.DefaultConfiguration.AutoSavePreAction = (context, audit) =>
            {
                using (var scope = serviceProvider.GetService<IServiceScopeFactory>().CreateScope())
                using (var db = scope.ServiceProvider.GetRequiredService<LogDbContext>())
                {
                    var entries = audit.Entries.Cast<CustomAuditEntry>();

                    db.AuditEntries.AddRange(entries);
                    db.SaveChanges();
                }
            };

            AuditManager.DefaultConfiguration.AuditEntryFactory = args =>
            {
                var sevice = serviceProvider.GetService<IHttpContextAccessor>();
                return new CustomAuditEntry()
                {
                    IpAddress = sevice.HttpContext.Connection.RemoteIpAddress.ToString(),
                    UserAgent = sevice.HttpContext.Request.Headers["User-Agent"].ToString()
                };
            };

            #endregion

            // регистрируем обработчик событи€ OnApplicationStarted
            applicationLifetime.ApplicationStarted.Register(OnApplicationStarted);
            applicationLifetime.ApplicationStopping.Register(OnApplicationStopping);
        }

        protected void OnApplicationStopping()
        {
            var _logger = (ServiceProvider.GetService<ILoggerFactory>()).CreateLogger("ApplicationStopping");
            var _db = ServiceProvider.GetService<AppDbContext>();

            _logger.LogWarning("Application is stopping...");
            foreach (var conn in _db.UserConnection.ToList())
            {
                conn.IsOnline = false;
            }

            _db.SaveChanges();

            try
            {
                var buffer = Encoding.UTF8.GetBytes("{\"messageType\":-1}");
                List<Task> list = new List<Task>();

                foreach (var ws in WebSocketMiddleware.WebSocketsList.Values)
                {
                    list.Add(ws.connection.CloseAsync(WebSocketCloseStatus.NormalClosure,"Server is shutdowning...",CancellationToken.None));                    
                }

                Task.WaitAll(list.ToArray());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        protected void OnApplicationStarted()
        {
            
        }
    }
}
