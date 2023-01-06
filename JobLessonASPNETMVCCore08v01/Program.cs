using Autofac;
using Autofac.Configuration;
using Autofac.Extensions.DependencyInjection;
using JobLessonASPNETMVCCore08v01.Extensions;
using JobLessonASPNETMVCCore08v01.Models.Reports;
using JobLessonASPNETMVCCore08v01.Services;
using JobLessonASPNETMVCCore08v01.Services.Impl;
using Microsoft.EntityFrameworkCore;
using Orders.DAL;
using Orders.DAL.Entities;

namespace JobLessonASPNETMVCCore08v01
{
    // Добавить пакеты: [1] TemplateEngine.Docx
    internal class Program
    {
        private static Random random = new Random();

        private static WebApplication? _app;

        public static WebApplication App
        {
            get
            {
                if (_app == null)
                {
                    _app = CreateHostBuilder(Environment.GetCommandLineArgs()).Build();

                    if (!_app.Environment.IsDevelopment())
                    {
                        _app.UseDeveloperExceptionPage();
                        //_app.UseExceptionHandler("/Home/Error");
                    }
                    _app.UseStaticFiles();

                    _app.UseRouting();

                    _app.UseAuthorization();

                    _app.MapControllerRoute(
                        name: "default",
                        pattern: "{controller=Home}/{action=Index}/{id?}");
                }
                return _app;
            }
            //_host ??= CreateHostBuilder(Environment.GetCommandLineArgs()).Build();
    }

        public static WebApplicationBuilder CreateHostBuilder(string[] args)
        {
            var webApplicationBuilder = WebApplication.CreateBuilder(args);
            webApplicationBuilder.Host
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureContainer<ContainerBuilder>(container => // Autofac
            {
                var config = new ConfigurationBuilder()
                        .AddJsonFile("autofac.config.json", true, false);
                var module = new ConfigurationModule(config.Build());
                var builder = new ContainerBuilder();
                builder.RegisterModule(module);
            })
            .ConfigureHostConfiguration(options =>
                options.AddJsonFile("appsettings.json"))
            .ConfigureAppConfiguration(options =>
                options.AddJsonFile("appsettings.json")
                .AddXmlFile("appsettings.xml", true)
                .AddIniFile("appsettings.ini", true)
                .AddEnvironmentVariables()
                .AddCommandLine(args))
            .ConfigureLogging(options =>
                options.ClearProviders()
                    .AddConsole()
                    .AddDebug())
            .ConfigureServices(ConfigureServices);

            return webApplicationBuilder;
        }
        private static void ConfigureServices(HostBuilderContext host, IServiceCollection services)
        {
            services.AddControllersWithViews();

            #region Register Base Services
            
            // Стандартный способ регистрации сервиса (Microsoft.Extensions.DependencyInjection)
            services.AddTransient<IOrderService, OrderService>();


            #endregion


            #region Configure EF DBContext Service (Orders Database)

            services.AddDbContext<OrdersDbContext>(options =>
            {
                options.UseSqlServer(host.Configuration["Settings:DatabaseOptions:ConnectionString"]);
            });

            #endregion
        }

        public static IServiceProvider Services => App.Services;

        static async Task Main(string[] args)
        {
            #region Create test Buyers if DB empty

            var dbContextOptionsBuilder = new DbContextOptionsBuilder<OrdersDbContext>()
                .UseSqlServer("data source = SAFRONIV-HONOR\\SQLEXPRESS; " +
                "initial catalog = OrdersDatabase3; User Id = OrdersUser; Password =12345;" +
                "MultipleActiveResultSets=True;App=EntityFramework; TrustServerCertificate=True");
            using (var context = new OrdersDbContext(dbContextOptionsBuilder.Options))
            {
                context.Database.EnsureCreated();
                if (!context.Buyers.Any())
                {
                    context.Buyers.Add(new Buyer
                    {
                        LastName = "Трофимов",
                        Name = "Алексей",
                        Patronymic = "Артёмович",
                        Birthday = DateTime.Now.AddYears(-23).Date,
                    });
                    context.Buyers.Add(new Buyer
                    {
                        LastName = "Зеленин",
                        Name = "Николай",
                        Patronymic = "Даниилович",
                        Birthday = DateTime.Now.AddYears(-36).Date,
                    });
                    context.Buyers.Add(new Buyer
                    {
                        LastName = "Ермаков",
                        Name = "Фёдор",
                        Patronymic = "Дмитриевич",
                        Birthday = DateTime.Now.AddYears(-19).Date,
                    });
                    context.Buyers.Add(new Buyer
                    {
                        LastName = "Смирнова",
                        Name = "Ангелина",
                        Patronymic = "Данииловна",
                        Birthday = DateTime.Now.AddYears(-31).Date,
                    });
                    context.Buyers.Add(new Buyer
                    {
                        LastName = "Белоусова",
                        Name = "Мария",
                        Patronymic = "Денисовна",
                        Birthday = DateTime.Now.AddYears(-26).Date,
                    });

                    context.SaveChanges();
                }
            }

            #endregion

            var app = App;
            await app.StartAsync();
            await PrintBuyersAsync();
            await app.StopAsync();
        }

        private static async Task PrintBuyersAsync()
        {
            await using (var servicesScope = Services.CreateAsyncScope())
            {
                var services = servicesScope.ServiceProvider;

                var logger = services.GetRequiredService<ILogger<Program>>();
                var context = services.GetRequiredService<OrdersDbContext>();

                //await context.Database.MigrateAsync();

                foreach (var buyer in context.Buyers)
                {
                    logger.LogInformation($"Покупатель >>> {buyer.Id} {buyer.LastName} {buyer.Name} {buyer.Patronymic} {buyer.Birthday.ToShortDateString()}");
                }

                var orderService = services.GetRequiredService<IOrderService>();


                await orderService.CreateAsync(random.Next(1, 6), "123, Russia, Address", "+79001112233", new (int, int)[] {
                    new ValueTuple<int, int>(1, 1)
                });


                //var catalog = new ProductsCatalog
                //{
                //    Name = "Каталог товаров",
                //    Description = "Актуальный список товаров на дату",
                //    CreationDate = DateTime.Now,
                //    Products = context.Products
                //};

                //string templateFile = "Templates/DefaultTemplate.docx";
                //IProductReport report = new ProductReportWord(templateFile);

                //CreateReport(report, catalog, "Report.docx");

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reportGenerator">Объект - генератор отчета</param>
        /// <param name="catalog">Объект с данными</param>
        /// <param name="reportFileName">Наименование файла-отчета</param>
        private static void CreateReport(IProductReport reportGenerator, ProductsCatalog catalog, string reportFileName)
        {
            reportGenerator.CatalogName = catalog.Name;
            reportGenerator.CatalogDescription = catalog.Description;
            reportGenerator.CreationDate = catalog.CreationDate;
            reportGenerator.Products = catalog.Products.Select(product => (product.Id, product.Name, product.Category, product.Price));

            var reportFileInfo = reportGenerator.Create(reportFileName);
            reportFileInfo.Execute();
        }

    }

}