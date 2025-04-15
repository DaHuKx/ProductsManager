using FluentValidation.Results;
using ProductsManager.Bots.Helpers;
using ProductsManager.Bots.Interfaces;
using ProductsManager.Bots.Models;
using ProductsManager.Bots.Validators;
using ProductsManager.Domain.Enums;
using ProductsManager.Infrastructure.DataBase.Enums;
using ProductsManager.Infrastructure.Repositories.Interfaces;
using System.Text;

namespace ProductsManager.Bots.MessageHandlers
{
    public class SetImportPriceHandler : IMessageHandler
    {
        private readonly IProductsRepository _productsRepository;

        public UserPlace Place => UserPlace.SetImport;

        public SetImportPriceHandler(IProductsRepository productsRepository)
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

            PriceValidation validator = new PriceValidation();
            ValidationResult result;

            var prices = message.Message.Split('\n');

            foreach (var price in prices)
            {
                result = validator.Validate(price);

                if (!result.IsValid)
                {
                    sb.AppendLine($"{price} - Ошибка валидации 🚫");
                    continue;
                }

                var temp = price.Split(' ');

                var id = int.Parse(temp[0]);
                var newPrice = decimal.Parse(temp[1]);

                var product = await _productsRepository.GetAsync(id);

                if (product is null)
                {
                    sb.AppendLine($"{id} {newPrice} - Продукт не найден 🚫");
                    continue;
                }

                if (newPrice <= 0)
                {
                    sb.AppendLine($"{id} {newPrice} - Цена не может быть меньше 1 🚫");
                    continue;
                }

                if (product.ExportPrice != null &&
                    product.ExportPrice < newPrice)
                {
                    sb.AppendLine($"{id} {newPrice} - Цена продажи продукта не может быть меньше цены закупки ({product.ExportPrice}p)");
                    continue;
                }

                if (await _productsRepository.UpdateProductPrice(id, newPrice, TradeType.Import) is null)
                {
                    sb.AppendLine($"{id} {newPrice} - Ошибка при добавлении в базу данных 🚫");
                }
                else
                {
                    sb.AppendLine($"{product.Name} - Новая цена {newPrice}р ✅");
                }
            }

            return new BotMessage
            {
                BotType = message.BotType,
                Message = sb.ToString(),
                UserId = message.UserId,
                KeyboardTexts = UserMessagesConsts.GetBackMessage()
            };
        }
    }
}
