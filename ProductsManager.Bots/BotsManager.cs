using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProductsManager.Bots.Clients;
using ProductsManager.Bots.Helpers;
using ProductsManager.Bots.Interfaces;
using ProductsManager.Bots.Models;
using ProductsManager.Infrastructure.DataBase.Entities;
using ProductsManager.Infrastructure.DataBase.Enums;
using ProductsManager.Infrastructure.Repositories.Interfaces;

namespace ProductsManager.Bots
{
    public class BotsManager
    {
        private readonly ILogger<BotsManager> _logger;
        private readonly IOptionsMonitor<BotsSettings> _optionsMonitor;
        private readonly IUsersRepository _botRepository;
        private readonly BotMessageResolver _botMessageResolver;

        private Dictionary<BotType, IBot> _bots;

        public BotsManager(IUsersRepository botRepository,
                           BotMessageResolver botMessageResolver,
                           IOptionsMonitor<BotsSettings> optionsMonitor,
                           ILogger<BotsManager> logger)
        {
            _optionsMonitor = optionsMonitor;
            _botRepository = botRepository;
            _botMessageResolver = botMessageResolver;
            _logger = logger;

            _bots = new Dictionary<BotType, IBot>
            {
                { BotType.Vk, new VkBot(_optionsMonitor.CurrentValue.VkAccessToken,
                                        _optionsMonitor.CurrentValue.VkGroupId,
                                        _logger) }
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
            _logger.LogInformation($"New message: UserID:{message.UserId}\n BotType:{message.BotType}\n Message:{message.Message}\n\n");

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
                    _logger.LogError($"User didn't added in DataBase - Type:{message.BotType}, Id:{message.UserId}");
                    return;
                }
            }

            BotMessage answer;

            var expectedMessages = UserMessagesConsts.GetExpectedMessages(user.Place);

            if (expectedMessages is not null &&
                expectedMessages.Count > 0 &&
               !expectedMessages.Contains(message.Message) &&
               !UserMessagesConsts.IsPlaceWithWriting(user.Place))
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
