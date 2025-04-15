using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProductsManager.Bots;
using ProductsManager.Bots.Interfaces;
using ProductsManager.Bots.MessageHandlers;
using ProductsManager.Bots.Reporters;
using ProductsManager.Business.Services;
using ProductsManager.Business.Services.Interfaces;
using ProductsManager.Infrastructure.DataBase;
using ProductsManager.Infrastructure.Repositories;
using ProductsManager.Infrastructure.Repositories.Interfaces;

IConfiguration configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                                         .AddJsonFile("appsettings.json")
                                                         .Build();

var serviceProvider = new ServiceCollection();

serviceProvider.Configure<BotsSettings>(configuration.GetSection("BotsSettings"));

serviceProvider.AddLogging(ConfigureLogging);

serviceProvider.AddDbContext<ProductsManagerDb>();

serviceProvider.AddSingleton<IProductsService, ProductsService>();

serviceProvider.AddTransient<IProductsRepository, ProductsRepository>();
serviceProvider.AddTransient<IUsersRepository, UsersRepository>();

serviceProvider.AddSingleton<BotsManager>();
serviceProvider.AddSingleton<BotMessageResolver>();

serviceProvider.AddTransient<ProductsReporter>();

serviceProvider.AddSingleton<IMessageHandler, ReportsMessageHandler>();
serviceProvider.AddSingleton<IMessageHandler, MenuMessageHandler>();
serviceProvider.AddSingleton<IMessageHandler, StartMessageHandler>();
serviceProvider.AddSingleton<IMessageHandler, AddProductsMessageHandler>();
serviceProvider.AddSingleton<IMessageHandler, AddExportsMessageHandler>();
serviceProvider.AddSingleton<IMessageHandler, AddImportsMessageHandler>();
serviceProvider.AddSingleton<IMessageHandler, SetImportPriceHandler>();
serviceProvider.AddSingleton<IMessageHandler, SetExportPriceHandler>();


var build = serviceProvider.BuildServiceProvider();

var app = build.GetService<BotsManager>();

await app!.RunBots();

void ConfigureLogging(ILoggingBuilder configure)
{
    configure.ClearProviders();
    configure.AddConsole();

    configure.SetMinimumLevel(LogLevel.Information);
}

//using ProductsManager.Bots.Reporters;
//using ProductsManager.Infrastructure.DataBase;
//using ProductsManager.Infrastructure.Repositories;

//ProductsRepository productsRepository = new ProductsRepository(new ProductsManagerDb());

//var trades = await productsRepository.GetTradesWithProductsAsync();

//ProductsReporter productsReporter = new ProductsReporter();
//TradesReporter tradesReporter = new TradesReporter();

//Console.WriteLine(tradesReporter.CreateTradesReport(trades));


