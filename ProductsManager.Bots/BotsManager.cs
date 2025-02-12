using ProductsManager.Bots.Clients;
using ProductsManager.Bots.Helpers;
using ProductsManager.Bots.Interfaces;
using ProductsManager.Bots.Models;
using ProductsManager.Bots.Properties;
using ProductsManager.Infrastructure.DataBase.Entities;
using ProductsManager.Infrastructure.DataBase.Enums;
using ProductsManager.Infrastructure.Repositories.Interfaces;

namespace ProductsManager.Bots
{
    public class BotsManager
    {
        private readonly IUsersRepository _botRepository;
        private readonly BotMessageResolver _botMessageResolver;

        private Dictionary<BotType, IBot> _bots;

        public BotsManager(IUsersRepository botRepository,
                           BotMessageResolver botMessageResolver)
        {
            _botRepository = botRepository;
            _botMessageResolver = botMessageResolver;

            _bots = new Dictionary<BotType, IBot>
            {
                { BotType.Vk, new VkBot(Resources.VkAccessToken) }
            };

            foreach (var bot in _bots.Values)
            {
                bot.OnGotMessage += NewMessageAsync;
            }
        }

        public async Task RunBots()
        {
            List<Task> tasks = new List<Task>();

            foreach (var bot in _bots.Values)
            {
                tasks.Add(bot.StartListeningMessages());
            }

            await Task.WhenAll(tasks);
        }

        private async void NewMessageAsync(BotMessage message)
        {
            var user = _botRepository.GetByNetId(message.BotType, message.UserId);

            if (user is null)
            {
                user = _botRepository.Add(new BotUser
                {
                    BotType = message.BotType,
                    NetId = message.UserId,
                    Place = UserPlace.Start
                });

                if (user is null)
                {
                    //TODO: Logger
                    return;
                }
            }

            BotMessage answer;

            var expectedMessages = UserMessagesConsts.GetExpectedMessages(user.Place);

            if (
                expectedMessages is not null &&
                expectedMessages.Count > 0 &&
               !expectedMessages.Contains(message.Message))
            {
                answer = new BotMessage
                {
                    UserId = message.UserId,
                    BotType = message.BotType,
                    Message = BotAnswersConsts.CreateErrorMessage(expectedMessages),
                    KeyboardTexts = expectedMessages
                };

                await _bots[user.BotType].SendMessageAsync(answer);

                return;
            }

            answer = await _botMessageResolver.ProcessMessage(user, message);

            if (answer.NewUserPlace is not null)
            {
                await _botRepository.UpdateUserPlaceAsync(user, answer.NewUserPlace.Value);
            }

            await _bots[user.BotType].SendMessageAsync(answer);
        }
    }
}
