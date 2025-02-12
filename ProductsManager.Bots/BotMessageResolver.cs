using FluentValidation.Results;
using ProductsManager.Bots.Helpers;
using ProductsManager.Bots.Models;
using ProductsManager.Bots.Validators;
using ProductsManager.Domain.DbEntities;
using ProductsManager.Domain.Enums;
using ProductsManager.Infrastructure.DataBase.Entities;
using ProductsManager.Infrastructure.DataBase.Enums;
using ProductsManager.Infrastructure.Repositories.Interfaces;
using System.Text;

namespace ProductsManager.Bots
{
    public class BotMessageResolver
    {
        private readonly IProductsRepository _productsRepository;

        private Dictionary<UserPlace, Func<BotMessage, Task<BotMessage>>> _placeMethods;

        public BotMessageResolver(IProductsRepository productsRepository)
        {
            _placeMethods = new Dictionary<UserPlace, Func<BotMessage, Task<BotMessage>>>
            {
                [UserPlace.Start] = UserStartMessage,
                [UserPlace.Menu] = UserMenuMessage,
                [UserPlace.AddProduct] = UserAddProductMessage,
                [UserPlace.AddImport] = UserAddImportMessage,
                [UserPlace.AddExport] = UserAddExportMessage
            };

            _productsRepository = productsRepository;
        }

        public async Task<BotMessage> ProcessMessage(BotUser user, BotMessage message)
        {
            return await _placeMethods[user.Place](message);
        }

        private async Task<BotMessage> UserStartMessage(BotMessage message)
        {
            return new BotMessage
            {
                Message = BotAnswersConsts.Menu,
                KeyboardTexts = UserMessagesConsts.GetExpectedMessages(UserPlace.Menu),
                NewUserPlace = UserPlace.Menu,
                UserId = message.UserId,
                BotType = message.BotType
            };
        }

        private async Task<BotMessage> UserMenuMessage(BotMessage message)
        {
            switch (message.Message)
            {
                case UserMessagesConsts.GetProducts:

                    var products = await _productsRepository.GetAllProductsWithTradesAsync();

                    return new BotMessage
                    {
                        BotType = message.BotType,
                        KeyboardTexts = UserMessagesConsts.GetExpectedMessages(UserPlace.Menu),
                        Message = products.Any() ? CreateMessageFromProducts(products) : BotAnswersConsts.NoProducts,
                        UserId = message.UserId
                    };

                case UserMessagesConsts.AddProduct:

                    return new BotMessage
                    {
                        BotType = message.BotType,
                        KeyboardTexts = UserMessagesConsts.GetBackMessage(),
                        NewUserPlace = UserPlace.AddProduct,
                        UserId = message.UserId,
                        Message = BotAnswersConsts.AddProduct
                    };

                case UserMessagesConsts.AddImport:

                    return new BotMessage
                    {
                        BotType = message.BotType,
                        KeyboardTexts = UserMessagesConsts.GetBackMessage(),
                        NewUserPlace = UserPlace.AddImport,
                        UserId = message.UserId,
                        Message = BotAnswersConsts.AddImport
                    };

                case UserMessagesConsts.AddExport:

                    return new BotMessage
                    {
                        BotType = message.BotType,
                        KeyboardTexts = UserMessagesConsts.GetBackMessage(),
                        NewUserPlace = UserPlace.AddExport,
                        UserId = message.UserId,
                        Message = BotAnswersConsts.AddExport
                    };
            }

            return new BotMessage();
        }

        private async Task<BotMessage> UserAddProductMessage(BotMessage message)
        {
            if (IsBackMessage(message, out BotMessage? answer))
            {
                return answer!;
            }

            StringBuilder stringBuilder = new StringBuilder();

            var products = message.Message.Split('\n');

            foreach (var product in products)
            {
                var result = await _productsRepository.AddAsync(new Product
                {
                    Name = product.Trim()
                });

                stringBuilder.AppendLine($"{(result is null ? product + " - Не удалось добавить 🚫" : result.Name + " - Добавлено успешно ✅")}");
            }

            return new BotMessage
            {
                BotType = message.BotType,
                KeyboardTexts = UserMessagesConsts.GetExpectedMessages(UserPlace.AddProduct),
                Message = stringBuilder.ToString(),
                UserId = message.UserId
            };
        }

        private async Task<BotMessage> UserAddImportMessage(BotMessage message)
        {
            if (IsBackMessage(message, out BotMessage? answer))
            {
                return answer!;
            }

            StringBuilder sb = new StringBuilder();

            var trades = message.Message.Split('\n');

            TradeValidation validations = new TradeValidation();
            ValidationResult result;

            foreach (var trade in trades)
            {
                result = validations.Validate(trade);

                if (!result.IsValid)
                {
                    sb.AppendLine($"{trade} - {result.Errors.First()} 🚫");
                    continue;
                }

                var numbers = trade.Split(' ');

                var id = int.Parse(numbers[0]);
                var count = int.Parse(numbers[1]);

                var product = await _productsRepository.AddTradeAsync(new Trade
                {
                    ProductId = id,
                    Count = count,
                    TimeStamp = DateTime.UtcNow,
                    Type = TradeType.Import
                });

                if (product != null)
                {
                    sb.AppendLine($"{product.Product!.Name} + {count}шт. Успешно ✅");
                }
                else
                {
                    sb.AppendLine($"{id} {count} - Ошибка во время добавления в базу 🚫");
                }
            }

            return new BotMessage
            {
                BotType = message.BotType,
                KeyboardTexts = UserMessagesConsts.GetBackMessage(),
                Message = sb.ToString(),
                UserId = message.UserId
            };
        }

        private async Task<BotMessage> UserAddExportMessage(BotMessage message)
        {
            if (IsBackMessage(message, out BotMessage? answer))
            {
                return answer!;
            }

            StringBuilder sb = new StringBuilder();

            var trades = message.Message.Split("\n");

            TradeValidation validation = new TradeValidation();
            ValidationResult result;

            foreach (var trade in trades)
            {
                result = validation.Validate(trade);

                if (!result.IsValid)
                {
                    sb.AppendLine($"{trade} - {result.Errors.First()} 🚫");
                    continue;
                }

                var numbers = trade.Split(' ');

                int id = int.Parse(numbers[0]);
                int count = int.Parse(numbers[1]);

                var product = await _productsRepository.GetProductWithTradesAsync(id);

                if (product is null)
                {
                    sb.AppendLine($"{id} {count} - Продукт не найден 🚫");
                    continue;
                }

                var stock = CalculateStockCount(product.Trades);

                if (stock < count)
                {
                    sb.AppendLine($"{product.Name} -{count}шт. - Товаров на складе не хватает ({stock}шт.) 🚫");
                    continue;
                }

                var res = await _productsRepository.AddTradeAsync(new Trade
                {
                    Count = count,
                    ProductId = id,
                    TimeStamp = DateTime.UtcNow,
                    Type = TradeType.Export
                });

                if (res is null)
                {
                    sb.AppendLine($"{product.Name} {count} - Ошибка при добавлении в базу 🚫");
                    continue;
                }

                sb.AppendLine($"{product.Name} -{count}шт. - Добавлено успешно ✅");
            }

            return new BotMessage
            {
                BotType = message.BotType,
                KeyboardTexts = UserMessagesConsts.GetBackMessage(),
                Message = sb.ToString(),
                UserId = message.UserId
            };
        }

        private string CreateMessageFromProducts(List<Product> products)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("Id продукта - Название:\n");

            foreach (var product in products)
            {
                stringBuilder.AppendLine($"{product.Id} - {product.Name}, {CalculateStockCount(product.Trades)}шт.");
            }

            return stringBuilder.ToString();
        }

        private int CalculateStockCount(List<Trade>? trades)
        {
            if (trades is null || !trades.Any())
            {
                return 0;
            }

            return trades.Where(t => t.Type == TradeType.Import).Sum(t => t.Count) - trades.Where(t => t.Type == TradeType.Export)
                                                                                           .Sum(t => t.Count);
        }

        private bool IsBackMessage(BotMessage message, out BotMessage? answer)
        {
            if (message.Message == UserMessagesConsts.Back)
            {
                answer = new BotMessage
                {
                    BotType = message.BotType,
                    KeyboardTexts = UserMessagesConsts.GetExpectedMessages(UserPlace.Menu),
                    Message = BotAnswersConsts.Menu,
                    NewUserPlace = UserPlace.Menu,
                    UserId = message.UserId
                };

                return true;
            }

            answer = null;

            return false;
        }
    }
}
