using ProductsManager.Bots.Helpers;
using ProductsManager.Bots.Interfaces;
using ProductsManager.Bots.Models;
using ProductsManager.Infrastructure.DataBase.Enums;
using ProductsManager.Infrastructure.Repositories.Interfaces;

namespace ProductsManager.Bots.MessageHandlers
{
    public sealed class MenuMessageHandler : IMessageHandler
    {
        private readonly IProductsRepository _productsRepository;

        public UserPlace Place => UserPlace.Menu;

        public MenuMessageHandler(IProductsRepository productsRepository)
        {
            _productsRepository = productsRepository;
        }

        public async Task<BotMessage> ResolveMessage(BotMessage message)
        {
            switch (message.Message)
            {
                case UserMessagesConsts.Reports:
                    return CreateMessage(message, UserPlace.Reports, BotAnswersConsts.Reports);

                case UserMessagesConsts.AddProduct:
                    return CreateMessage(message, UserPlace.AddProduct, BotAnswersConsts.AddProduct);

                case UserMessagesConsts.AddImport:
                    return CreateMessage(message, UserPlace.AddImport, BotAnswersConsts.AddImport);

                case UserMessagesConsts.AddExport:
                    return CreateMessage(message, UserPlace.AddExport, BotAnswersConsts.AddExport);

                case UserMessagesConsts.SetExport:
                    return CreateMessage(message, UserPlace.SetExport, BotAnswersConsts.SetImportExport);

                case UserMessagesConsts.SetImport:
                    return CreateMessage(message, UserPlace.SetImport, BotAnswersConsts.SetImportExport);

                default:
                    return new BotMessage();
            }
        }

        private BotMessage CreateMessage(BotMessage userMessage, UserPlace newPlace, string message)
        {
            return new BotMessage
            {
                BotType = userMessage.BotType,
                KeyboardTexts = UserMessagesConsts.GetExpectedMessages(newPlace),
                NewUserPlace = newPlace,
                UserId = userMessage.UserId,
                Message = message
            };
        }
    }
}
