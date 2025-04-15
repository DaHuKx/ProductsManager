using ProductsManager.Bots.Helpers;
using ProductsManager.Bots.Interfaces;
using ProductsManager.Bots.Models;
using ProductsManager.Domain.DbEntities;
using ProductsManager.Infrastructure.DataBase.Enums;
using ProductsManager.Infrastructure.Repositories.Interfaces;
using System.Text;

namespace ProductsManager.Bots.MessageHandlers
{
    public sealed class AddProductsMessageHandler : IMessageHandler
    {
        private readonly IProductsRepository _productsRepository;

        public UserPlace Place => UserPlace.AddProduct;

        public AddProductsMessageHandler(IProductsRepository productsRepository)
        {
            _productsRepository = productsRepository;
        }

        public async Task<BotMessage> ResolveMessage(BotMessage message)
        {
            if (UserMessagesConsts.IsBackMessage(message, out BotMessage? answer))
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

                stringBuilder.AppendLine($"{(result is null ? product + " - Не удалось добавить 🚫"
                                                            : result.Name + $" - Добавлено, Id - {result.Id} ✅")}");
            }

            return new BotMessage
            {
                BotType = message.BotType,
                KeyboardTexts = UserMessagesConsts.GetBackMessage(),
                Message = stringBuilder.ToString(),
                UserId = message.UserId
            };
        }
    }
}
