//var serviceProvider = new ServiceCollection();

//serviceProvider.AddDbContext<ProductsManagerDb>();

//serviceProvider.AddSingleton<IProductsService, ProductsService>();

//serviceProvider.AddTransient<IProductsRepository, ProductsRepository>();
//serviceProvider.AddTransient<IUsersRepository, UsersRepository>();

//serviceProvider.AddSingleton<BotsManager>();
//serviceProvider.AddSingleton<BotMessageResolver>();

//var build = serviceProvider.BuildServiceProvider();

//var app = build.GetService<BotsManager>();

//await app!.RunBots();

using ProductsManager.Bots.Reporters;
using ProductsManager.Infrastructure.DataBase;
using ProductsManager.Infrastructure.Repositories;

ProductsRepository productsRepository = new ProductsRepository(new ProductsManagerDb());

var trades = await productsRepository.GetTradesWithProductsAsync();

ProductsReporter productsReporter = new ProductsReporter();
TradesReporter tradesReporter = new TradesReporter();

Console.WriteLine(tradesReporter.CreateTradesReport(trades));


