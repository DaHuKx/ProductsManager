using Microsoft.Extensions.Logging;
using ProductsManager.Bots.Helpers;
using ProductsManager.Bots.Interfaces;
using ProductsManager.Bots.Models;
using ProductsManager.Infrastructure.DataBase.Entities;
using ProductsManager.Infrastructure.DataBase.Enums;

namespace ProductsManager.Bots
{
    public sealed class BotMessageResolver : IBotMessageResolver
    {
        private readonly ILogger<BotMessageResolver> _logger;

        private Dictionary<UserPlace, IMessageHandler> _placeMethods;

        public BotMessageResolver(IEnumerable<IMessageHandler> handlers, ILogger<BotMessageResolver> logger)
        {
            _logger = logger;

            _placeMethods = handlers.ToDictionary(h => h.Place, h => h);
        }

        public async Task<BotMessage> ProcessMessage(BotUser user, BotMessage message)
        {
            if (_placeMethods.ContainsKey(user.Place))
            {
                try
                {
                    return await _placeMethods[user.Place].ResolveMessage(message);
                }
                catch
                {
                    _logger.LogError($"Method didn't found for user place: {user.Place}, UserId: {user.NetId}, BotType: {message.BotType}");
                }
            }

            return new BotMessage
            {
                BotType = message.BotType,
                KeyboardTexts = UserMessagesConsts.GetExpectedMessages(user.Place),
                UserId = message.UserId,
            };
        }
    }
}
