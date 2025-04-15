using FluentValidation.Results;
using ProductsManager.Bots.Helpers;
using ProductsManager.Bots.Interfaces;
using ProductsManager.Bots.Models;
using ProductsManager.Bots.Validators;
using ProductsManager.Domain.DbEntities;
using ProductsManager.Domain.Enums;
using ProductsManager.Infrastructure.DataBase.Enums;
using ProductsManager.Infrastructure.Repositories.Interfaces;
using System.Text;

namespace ProductsManager.Bots.MessageHandlers
{
    public class AddExportsMessageHandler : IMessageHandler
    {
        private readonly IProductsRepository _productsRepository;

        public UserPlace Place => UserPlace.AddExport;

        public AddExportsMessageHandler(IProductsRepository productsRepository)
        {
            _productsRepository = productsRepository;
        }

        public async Task<BotMessage> ResolveMessage(BotMessage message)
        {
            if (UserMessagesConsts.IsBackMessage(message, out BotMessage? answer))
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
                    sb.AppendLine($"{trade} - Ошибка валидации 🚫");
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
                    sb.AppendLine($"{product.Name} - {count}шт. - Товаров на складе не хватает ({stock}шт.) 🚫");
                    continue;
                }

                if (!product.ExportPrice.HasValue)
                {
                    sb.AppendLine($"{product.Id} {product.Name} - Цена продажи товара не указана. 🚫");
                }

                var res = await _productsRepository.AddTradeAsync(new Trade
                {
                    Count = count,
                    ProductId = id,
                    Price = product.ExportPrice!.Value,
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

        private int CalculateStockCount(List<Trade>? trades)
        {
            if (trades is null || !trades.Any())
            {
                return 0;
            }

            return trades.Where(t => t.Type == TradeType.Import).Sum(t => t.Count) - trades.Where(t => t.Type == TradeType.Export)
                                                                                           .Sum(t => t.Count);
        }
    }
}
