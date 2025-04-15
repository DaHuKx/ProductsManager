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
    public class AddImportsMessageHandler : IMessageHandler
    {
        private readonly IProductsRepository _productsRepository;

        public UserPlace Place => UserPlace.AddImport;

        public AddImportsMessageHandler(IProductsRepository productsRepository)
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

            var trades = message.Message.Split('\n');

            TradeValidation validations = new TradeValidation();
            ValidationResult result;

            foreach (var trade in trades)
            {
                result = validations.Validate(trade);

                if (!result.IsValid)
                {
                    sb.AppendLine($"{trade} - Ошибка валидации 🚫");
                    continue;
                }

                var numbers = trade.Split(' ');

                var id = int.Parse(numbers[0]);
                var count = int.Parse(numbers[1]);

                var product = await _productsRepository.GetAsync(id);

                if (product is null)
                {
                    sb.AppendLine($"{id} {count} - Продукт не найден 🚫");
                    continue;
                }

                if (!product.ImportPrice.HasValue)
                {
                    sb.AppendLine($"{product.Name} - Цена закупки не указана 🚫");
                    continue;
                }

                var tradeResult = await _productsRepository.AddTradeAsync(new Trade
                {
                    ProductId = id,
                    Count = count,
                    Price = product.ImportPrice!.Value,
                    TimeStamp = DateTime.UtcNow,
                    Type = TradeType.Import
                });

                if (tradeResult != null)
                {
                    sb.AppendLine($"{tradeResult.Product!.Name} + {count}шт. Успешно ✅");
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
    }
}
