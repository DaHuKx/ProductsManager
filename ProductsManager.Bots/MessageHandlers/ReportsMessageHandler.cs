using ProductsManager.Bots.Helpers;
using ProductsManager.Bots.Interfaces;
using ProductsManager.Bots.Models;
using ProductsManager.Bots.Reporters;
using ProductsManager.Infrastructure.DataBase.Enums;
using ProductsManager.Infrastructure.Repositories.Interfaces;

namespace ProductsManager.Bots.MessageHandlers
{
    public class ReportsMessageHandler : IMessageHandler
    {
        private readonly ProductsReporter _reporter;
        private readonly IProductsRepository _productsRepository;

        public UserPlace Place => UserPlace.Reports;

        public ReportsMessageHandler(ProductsReporter reporter,
                                     IProductsRepository productsRepository)
        {

            _reporter = reporter;
            _productsRepository = productsRepository;
        }

        public async Task<BotMessage> ResolveMessage(BotMessage message)
        {
            if (UserMessagesConsts.IsBackMessage(message, out BotMessage? answer))
            {
                return answer!;
            }

            var expectedMessages = UserMessagesConsts.GetExpectedMessages(Place);

            var botAnswer = new BotMessage
            {
                BotType = message.BotType,
                KeyboardTexts = expectedMessages,
                UserId = message.UserId
            };

            switch (message.Message)
            {
                case UserMessagesConsts.StorageReport:

                    var products = await _productsRepository.GetAllProductsWithTradesAsync();

                    botAnswer.DocumentPath = _reporter.CreateProductsReport(products);
                    botAnswer.Message = "Успешно!";

                    break;

                default:
                    botAnswer.Message = BotAnswersConsts.CreateErrorMessage(expectedMessages!);
                    break;
            }

            return botAnswer;
        }
    }
}
