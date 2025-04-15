using ProductsManager.Bots.Helpers;
using ProductsManager.Bots.Interfaces;
using ProductsManager.Bots.Models;
using ProductsManager.Infrastructure.DataBase.Enums;

namespace ProductsManager.Bots.MessageHandlers
{
    public class StartMessageHandler : IMessageHandler
    {
        public UserPlace Place => UserPlace.Start;

        public async Task<BotMessage> ResolveMessage(BotMessage message)
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
    }
}
