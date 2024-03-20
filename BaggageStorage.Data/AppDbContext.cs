using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using BaggageStorage.Data.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using static BaggageStorage.Data.Models.PermissionEnums;
using Microsoft.CodeAnalysis;

namespace BaggageStorage.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        private readonly IHostingEnvironment env;
        private readonly IConfigurationRoot config;

        public DbSet<Customer> Customers { get; set; }
        public DbSet<WorkPlace> WorkPlaces { get; set; }
        public DbSet<StoragePlace> StoragePlaces { get; set; }
        public DbSet<Storage> Storages { get; set; }
        public DbSet<BaggageMoving> BaggageMovings { get; set; }
        public DbSet<BaggageMovingArchive> BaggageMovingArchives { get; set; }
        public DbSet<OperationType> OperationTypes { get; set; }
        public DbSet<Operation> Operations { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<MainMenu> MainMenu { get; set; }
        public DbSet<UserConnection> UserConnection { get; set; }
        public DbSet<CashOperation> CashOperations { get; set; }
        public DbSet<CashOperationArchive> CashOperationArchives { get; set; }
        public DbSet<Client> Clients { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public AppDbContext(DbContextOptions<AppDbContext> options, IHostingEnvironment env) : base(options)
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

            #region IdentityFramework
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable("Users");
                entity
                    .Ignore(i => i.PhoneNumber)
                    .Ignore(i => i.PhoneNumberConfirmed)
                    .Ignore(i => i.TwoFactorEnabled)
                    .Ignore(i => i.AccessFailedCount)
                    .Ignore(i => i.ConcurrencyStamp);
            });

            modelBuilder.Entity<ApplicationRole>(entity =>
            {
                entity.ToTable("Roles");
            });

            modelBuilder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.ToTable("RoleClaims");
            });

            modelBuilder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable("UserClaims");
            });

            modelBuilder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("UserRoles");
            });

            modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable("UserLogins");
            });

            modelBuilder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.ToTable("UserTokens");
            });
            #endregion

            modelBuilder.Entity<Permission>()
                .HasIndex(t => new { t.RoleId, t.OperationId })
                .IsUnique();

            modelBuilder.Entity<BaggageMoving>()
                .HasOne<ApplicationUser>(s => s.UserIn)
                .WithMany(g => g.UsersIn)
                .HasForeignKey(s => s.UserInId);

            modelBuilder.Entity<BaggageMoving>()
                .HasOne<ApplicationUser>(s => s.UserOut)
                .WithMany(g => g.UsersOut)
                .HasForeignKey(s => s.UserOutId);

            modelBuilder.Entity<Client>()
            .Property(f => f.OrderId)
            .ValueGeneratedOnAdd();
        }



        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            var dbType = config["AppConfigurations:DatabaseType"];

            switch (dbType)
            {
                case "mssql":
                    optionsBuilder.UseSqlServer(config.GetConnectionString("MainMsSql")); ;
                    break;
                case "postgres":
                    optionsBuilder.UseNpgsql(config.GetConnectionString("MainPostgres")); ;
                    break;
                case "sqlite":
                    optionsBuilder.UseSqlite(config.GetConnectionString("MainSqLite")); ;
                    break;
                case "sqlcompact":
                    optionsBuilder.UseSqlCe(config.GetConnectionString("MainSqlCompact")); ;
                    break;
                default:
                    throw new Exception("DatabaseType mast be mssql or postgres in appsettings.json");
            }

            base.OnConfiguring(optionsBuilder);
        }

        async public Task EnsureSeedData(IServiceProvider applicationServices)
        {
            UserManager<ApplicationUser> userManager = applicationServices.GetRequiredService<UserManager<ApplicationUser>>();
            RoleManager<ApplicationRole> roleManager = applicationServices.GetRequiredService<RoleManager<ApplicationRole>>();
            ILoggerFactory loggerFactory = applicationServices.GetRequiredService<ILoggerFactory>();

            var logger = loggerFactory.CreateLogger("EnsureSeedData");

            var developer = Customers.FirstOrDefault(x => x.Email == "developer@test.com");
            if (developer == null)
            {
                var id = Guid.NewGuid().ToString().ToLower();

                developer = new Customer
                {
                    Id = id,
                    ParentId = id,
                    Name = "Developer",
                    Email = "developer@test.com",
                    Address = ""
                };

                Customers.Add(developer);

                SaveChanges();
            }

            // смотрим, есть ли пользователь Developer
            var userDeveloper = await userManager.FindByEmailAsync("developer@test.com");
            if (userDeveloper == null)
            {
                var developerId = Guid.NewGuid().ToString().ToLower();
                var createUserResult = await userManager.CreateAsync(new ApplicationUser
                {
                    Id = developerId,
                    CustomerId = developer.Id,
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    LockoutEnd = DateTime.Now,
                    Email = "developer@test.com",
                    FirstName = "Admin",
                    LastName = "Admin",
                    UserName = "developer",
                }, "1111");

                if (!createUserResult.Succeeded)
                {
                    logger.LogError(String.Join("\r\n", createUserResult.Errors));
                    return;
                }
                else
                    userDeveloper = await userManager.FindByEmailAsync("developer@test.com");
            }




            // смотрим есть ли роль Developer
            var developerRole = await roleManager.FindByNameAsync("developer");
            if (developerRole == null)
            {
                var createRoleResult = await roleManager.CreateAsync(new ApplicationRole
                {
                    CustomerId = developer.Id,
                    Name = "developer",
                    Alias = "developer",
                    UserId = userDeveloper.Id
                });
                if (!createRoleResult.Succeeded)
                {
                    logger.LogError(String.Join("\r\n", createRoleResult.Errors));
                    return;
                }
                else developerRole = await roleManager.FindByNameAsync("developer");
            }




            // операции заполняем
            var operType = OperationTypes.FirstOrDefault(s => s.Id == "adcf9d16-285c-496c-b8a9-ddb5d972f1a9");
            if (operType == null)
            {
                OperationTypes.Add(new OperationType { Id = "adcf9d16-285c-496c-b8a9-ddb5d972f1a9", Name = "Authorization" });
                SaveChanges();
                operType = OperationTypes.FirstOrDefault(s => s.Id == "adcf9d16-285c-496c-b8a9-ddb5d972f1a9");
            }

            var operSignIn = Operations.FirstOrDefault(s => s.Id == "1c767063-ad79-48fb-b853-5218eddf0ae8");
            if (operSignIn == null)
            {
                Operations.Add(new Operation
                {
                    Id = "1c767063-ad79-48fb-b853-5218eddf0ae8",
                    EnumName = "Signin",
                    Name = "Вход в систему",
                    OperationTypeId = operType.Id
                });
                SaveChanges();
                operSignIn = Operations.FirstOrDefault(s => s.Id == "1c767063-ad79-48fb-b853-5218eddf0ae8");
            }

            var permissionSignIn = Permissions.FirstOrDefault(s => s.OperationId == operSignIn.Id && s.RoleId == developerRole.Id);
            if (permissionSignIn == null)
            {
                Permissions.Add(new Permission
                {
                    OperationId = operSignIn.Id,
                    RoleId = developerRole.Id,
                    Permitted = true
                });
                SaveChanges();
            }

            if (!(await userManager.IsInRoleAsync(userDeveloper, "developer")))
                await userManager.AddToRoleAsync(userDeveloper, "developer");

            if (MainMenu.Count() == 0)
                CreateMainMenu();

            SeedMenuAccessOperations();
        }

        private void CreateMainMenu()
        {
            // пункт меню Разработчик
            MainMenu developerItem = new Models.MainMenu
            {
                ParentId = 0,
                Text = "Владелец системы",
                TextEn = "Владелец системы",
                TextRo = "Владелец системы",
                OrderId = 100,
                Icon = "icon-diamond",
                PermissionEnumText = Enum.GetName(typeof(PermissionEnum), PermissionEnum.Menu_Owner),
                IsActive = true
            };

            MainMenu.Add(developerItem);
            SaveChanges();

            MainMenu.Add(new Models.MainMenu            // Клиенты системы
            {
                ParentId = developerItem.Id,
                Text = "Клиенты системы",
                TextEn = "Клиенты системы",
                TextRo = "Клиенты системы",
                Icon = "fa fa-group",
                JsFunction = "ShowView('/customer','customer',2)",
                OrderId = 0,
                PermissionEnumText = Enum.GetName(typeof(PermissionEnum), PermissionEnum.Menu_Owner),
                IsActive = true
            });

            MainMenu.Add(new Models.MainMenu            // Группы операций (типы операций)
            {
                ParentId = developerItem.Id,
                Text = "Группы операций",
                TextRo = "Группы операций",
                TextEn = "Группы операций",
                Icon = "fa fa-cube",
                JsFunction = "ShowView('/operationtype','operationtype',3)",
                OrderId = 1,
                PermissionEnumText = Enum.GetName(typeof(PermissionEnum), PermissionEnum.Menu_Owner),
                IsActive = true
            });
            MainMenu.Add(new Models.MainMenu            // Операции
            {
                ParentId = developerItem.Id,
                Text = "Операции",
                TextEn = "Операции",
                TextRo = "Операции",
                Icon = "fa fa-cubes",
                JsFunction = "ShowView('/operation','operation',4)",
                OrderId = 2,
                PermissionEnumText = Enum.GetName(typeof(PermissionEnum), PermissionEnum.Menu_Owner),
                IsActive = true
            });


            MainMenu.Add(new Models.MainMenu            // Глобальное меню
            {
                ParentId = developerItem.Id,
                Text = "Глобальное меню",
                TextRo = "Глобальное меню",
                TextEn = "Глобальное меню",
                Icon = "fa fa-navicon",
                JsFunction = "ShowView('/mainmenu','mainmenu',5)",
                OrderId = 3,
                PermissionEnumText = Enum.GetName(typeof(PermissionEnum), PermissionEnum.Menu_Owner),
                IsActive = true
            });

            // пункт меню Администрирование
            MainMenu administrationItem = new Models.MainMenu
            {
                ParentId = 0,
                OrderId = 99,
                Text = "Администрирование",
                TextEn = "Администрирование",
                TextRo = "Администрирование",
                Icon = "icon-settings",
                PermissionEnumText = Enum.GetName(typeof(PermissionEnum), PermissionEnum.Menu_Administration),
                IsActive = true
            };

            MainMenu.Add(administrationItem);
            SaveChanges();

            MainMenu.Add(new Models.MainMenu            // Роли
            {
                ParentId = administrationItem.Id,
                Text = "Роли",
                TextEn = "Роли",
                TextRo = "Роли",
                Icon = "fa fa-user-md",
                JsFunction = "ShowView('/role','role',7)",
                OrderId = 0,
                PermissionEnumText = Enum.GetName(typeof(PermissionEnum), PermissionEnum.Menu_Roles),
                IsActive = true
            });

            MainMenu.Add(new Models.MainMenu            // Права доступа
            {
                ParentId = administrationItem.Id,
                Text = "Права доступа",
                TextEn = "Права доступа",
                TextRo = "Права доступа",
                Icon = "fa fa-key",
                JsFunction = "ShowView('/permission','permission',8)",
                OrderId = 1,
                PermissionEnumText = Enum.GetName(typeof(PermissionEnum), PermissionEnum.Menu_Permissions),
                IsActive = true
            });

            MainMenu.Add(new Models.MainMenu            // Пользователи
            {
                ParentId = administrationItem.Id,
                Text = "Пользователи",
                TextEn = "Пользователи",
                TextRo = "Пользователи",
                Icon = "icon-user",
                JsFunction = "ShowView('/user','user',9)",
                OrderId = 2,
                PermissionEnumText = Enum.GetName(typeof(PermissionEnum), PermissionEnum.Menu_Users),
                IsActive = true
            });







            MainMenu.Add(new Models.MainMenu            // Приём багажа
            {
                ParentId = 0,
                Text = "Приём багажа",
                TextEn = "Приём багажа",
                TextRo = "Приём багажа",
                Icon = "fa fa-lock",
                JsFunction = "ShowView('/baggagereceiving','baggagereceiving',10)",
                OrderId = 0,
                PermissionEnumText = Enum.GetName(typeof(PermissionEnum), PermissionEnum.Menu_BaggageIn),
                IsActive = true
            });

            MainMenu.Add(new Models.MainMenu            // Выдача багажа
            {
                ParentId = 0,
                Text = "Выдача багажа",
                TextEn = "Выдача багажа",
                TextRo = "Выдача багажа",
                Icon = "fa fa-unlock-alt",
                JsFunction = "ShowView('/baggagedelivery','baggagedelivery',11)",
                OrderId = 1,
                PermissionEnumText = Enum.GetName(typeof(PermissionEnum), PermissionEnum.Menu_BaggageOut),
                IsActive = true
            });









            MainMenu sectorsItem = new Models.MainMenu            // Рабочие сектора 
            {
                ParentId = 0,
                Text = "Рабочие сектора",
                TextEn = "Рабочие сектора",
                TextRo = "Рабочие сектора",
                Icon = "fa fa-sitemap",
                OrderId = 3,
                PermissionEnumText = Enum.GetName(typeof(PermissionEnum), PermissionEnum.Menu_References),
                IsActive = true
            };

            MainMenu.Add(sectorsItem);
            SaveChanges();


            MainMenu.Add(new Models.MainMenu            // Камеры хранения
            {
                ParentId = sectorsItem.Id,
                Text = "Камеры хранения",
                TextEn = "Камеры хранения",
                TextRo = "Камеры хранения",
                Icon = "fa fa-building",
                JsFunction = "ShowView('/baggagestorage','baggagestorage',13)",
                OrderId = 0,
                PermissionEnumText = Enum.GetName(typeof(PermissionEnum), PermissionEnum.Menu_BaggageStorage),
                IsActive = true
            });

            MainMenu.Add(new Models.MainMenu            // Места хранения
            {
                ParentId = sectorsItem.Id,
                Text = "Места хранения",
                TextEn = "Места хранения",
                TextRo = "Места хранения",
                Icon = "fa fa-th-large",
                JsFunction = "ShowView('/storageplaces','storageplaces',14)",
                OrderId = 1,
                PermissionEnumText = Enum.GetName(typeof(PermissionEnum), PermissionEnum.Menu_StoragePlaces),
                IsActive = true
            });

            MainMenu.Add(new Models.MainMenu            // Рабочие места
            {
                ParentId = sectorsItem.Id,
                Text = "Рабочие места",
                TextEn = "Рабочие места",
                TextRo = "Рабочие места",
                Icon = "icon-screen-desktop",
                JsFunction = "ShowView('/workplace','workplace',15)",
                OrderId = 2,
                PermissionEnumText = Enum.GetName(typeof(PermissionEnum), PermissionEnum.Menu_WorkingPlaces),
                IsActive = true
            });









            MainMenu reports = new Models.MainMenu            //Отчёты
            {
                ParentId = 0,
                Text = "Отчёты",
                TextEn = "Отчёты",
                TextRo = "Отчёты",
                Icon = "icon-docs",
                OrderId = 2,
                PermissionEnumText = Enum.GetName(typeof(PermissionEnum), PermissionEnum.Menu_Reports),
                IsActive = true
            };

            MainMenu.Add(reports);
            SaveChanges();


            MainMenu.Add(new Models.MainMenu                //Реестр багажа
            {
                ParentId = reports.Id,
                Text = "Реестр багажа",
                TextEn = "Реестр багажа",
                TextRo = "Реестр багажа",
                Icon = "fa fa-history",
                JsFunction = "ShowView('/baggageregister','baggageregister',17)",
                OrderId = 0,
                PermissionEnumText = Enum.GetName(typeof(PermissionEnum), PermissionEnum.Menu_RegistryBaggage),
                IsActive = true
            });

            MainMenu.Add(new Models.MainMenu                //Кассовые операции
            {
                ParentId = reports.Id,
                Text = "Кассовые операции",
                TextEn = "Кассовые операции",
                TextRo = "Кассовые операции",
                Icon = "fa fa-dollar",
                JsFunction = "ShowView('/cashoperation','cashoperation',18)",
                OrderId = 1,
                PermissionEnumText = Enum.GetName(typeof(PermissionEnum), PermissionEnum.Menu_CashOperations),
                IsActive = true
            });


            SaveChanges();
        }

        private void SeedMenuAccessOperations()
        {
            var operType = OperationTypes.FirstOrDefault(s => s.Name.Equals("Menu_access"));
            if (operType == null)
            {
                string guid = Guid.NewGuid().ToString();
                OperationTypes.Add(new OperationType { Id = guid, Name = "Menu_access" });
                SaveChanges();
                operType = OperationTypes.FirstOrDefault(s => s.Id == guid);
            }
            AddMenuAccesOperation("Доступ к меню \"История багажа\"", operType.Id, PermissionEnum.Menu_RegistryBaggage);
            AddMenuAccesOperation("Доступ к меню \"Администрирование\"", operType.Id, PermissionEnum.Menu_Administration);
            AddMenuAccesOperation("Доступ к меню \"Рабочие места\"", operType.Id, PermissionEnum.Menu_WorkingPlaces);
            AddMenuAccesOperation("Доступ к меню \"Места хранения\"", operType.Id, PermissionEnum.Menu_StoragePlaces);
            AddMenuAccesOperation("Доступ к меню \"Роли\"", operType.Id, PermissionEnum.Menu_Roles);
            AddMenuAccesOperation("Доступ к меню \"Пользователи\"", operType.Id, PermissionEnum.Menu_Users);
            AddMenuAccesOperation("Доступ к меню \"Камеры хранения\"", operType.Id, PermissionEnum.Menu_BaggageStorage);
            AddMenuAccesOperation("Доступ к меню \"Права доступа\"", operType.Id, PermissionEnum.Menu_Permissions);
            AddMenuAccesOperation("Доступ к меню \"Кассовые операции\"", operType.Id, PermissionEnum.Menu_CashOperations);
            AddMenuAccesOperation("Доступ к меню \"Приём багажа\"", operType.Id, PermissionEnum.Menu_BaggageIn);
            AddMenuAccesOperation("Доступ к меню \"Выдача багажа\"", operType.Id, PermissionEnum.Menu_BaggageOut);
            AddMenuAccesOperation("Доступ к меню \"Рабочие сектора\"", operType.Id, PermissionEnum.Menu_References);
            AddMenuAccesOperation("Доступ к меню \"Владелец системы\"", operType.Id, PermissionEnum.Menu_Owner);
            AddMenuAccesOperation("Доступ к меню \"Отчёты\"", operType.Id, PermissionEnum.Menu_Reports);
        }
        
        private void AddMenuAccesOperation(string menuName, string typeId, PermissionEnum permissionEnum)
        {
            var enumName = Enum.GetName(typeof(PermissionEnum), permissionEnum);
            var operation = Operations.FirstOrDefault(s => s.EnumName.Equals(enumName));
            if (operation == null)
            {
                string guid = Guid.NewGuid().ToString();
                Operations.Add(new Operation
                {
                    Id = guid,
                    EnumName = enumName,
                    Name = menuName,
                    OperationTypeId = typeId,
                });
                SaveChanges();
                operation = Operations.FirstOrDefault(s => s.Id == guid);

                foreach (var role in Roles.ToList())
                {
                    Permissions.Add(new Data.Models.Permission
                    {
                        Operation = operation,
                        Permitted = role.NormalizedName.Equals("DEVELOPER") ? true : false,
                        Role = role
                    });
                }
                SaveChanges();
            }
        }
    }
}
