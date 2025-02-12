using ProductsManager.Bots.Interfaces;
using ProductsManager.Bots.Models;
using ProductsManager.Bots.Properties;
using ProductsManager.Infrastructure.DataBase.Enums;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Enums.StringEnums;
using VkNet.Model;

namespace ProductsManager.Bots.Clients
{
    public class VkBot : BotBase, IBot
    {
        private readonly VkApi _api;
        private readonly Random _random;

        private ulong? _ts;

        public event BotMessageHandler? OnGotMessage;

        public VkBot()
        {
            BotType = BotType.Vk;

            _random = new Random();
            _api = new VkApi();
        }

        public VkBot(string accessToken) : this()
        {
            _api!.Authorize(new ApiAuthParams
            {
                AccessToken = accessToken,
                Settings = Settings.All
            });

            _ = Task.Run(() => SendMessageAsync(new BotMessage
            {
                BotType = BotType.Vk,
                Message = "Запущен.",
                UserId = "322039043"
            }));
        }

        public async Task Initialize(string accessToken)
        {
            await _api.AuthorizeAsync(new ApiAuthParams
            {
                AccessToken = accessToken,
                Settings = Settings.All
            });
        }

        public async Task SendMessageAsync(BotMessage message)
        {
            if (!_api.IsAuthorized)
            {
                //TODO: Logger
                return;
            }

            await _api.Messages.SendAsync(new MessagesSendParams
            {
                Message = message.Message,
                UserId = long.Parse(message.UserId),
                RandomId = _random.Next(),
                Keyboard = message.KeyboardTexts is null ? default : CreateKeyboard(message.KeyboardTexts)
            });
        }

        public async Task StartListeningMessages()
        {
            while (true)
            {
                try
                {
                    var response = GetBotsLongPollHistoryResponse();

                    if (response.Updates is null || response.Updates.Count == 0)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }

                    foreach (var update in response.Updates.Where(u => u.Type.Value == GroupUpdateType.MessageNew))
                    {
                        CheckUpdate(update);
                    }
                }
                catch
                {
                    //TODO: Logger.
                }

                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

        private void CheckUpdate(GroupUpdate update)
        {
            if (update.Type.Value != GroupUpdateType.MessageNew) return;

            var message = update.Instance as MessageNew;

            if (message is null) return;

            OnGotMessage?.Invoke(new BotMessage
            {
                BotType = BotType.Vk,
                Message = message.Message.Text,
                UserId = message.Message.FromId!.Value.ToString()
            });
        }

        private BotsLongPollHistoryResponse GetBotsLongPollHistoryResponse()
        {
            var pollResponse = _api.Groups.GetLongPollServer(ulong.Parse(Resources.VkGroupId));

            BotsLongPollHistoryResponse response;

            response = _api.Groups.GetBotsLongPollHistory(new BotsLongPollHistoryParams
            {
                Server = pollResponse.Server,
                Ts = _ts ?? pollResponse.Ts,
                Key = pollResponse.Key,
                Wait = 90
            });

            _ts = response.Ts;

            return response;
        }

        private MessageKeyboard CreateKeyboard(List<string> texts)
        {
            AddButtonParams GetButton(string text)
            {
                return new AddButtonParams
                {
                    Label = text,
                    Color = KeyboardButtonColor.Primary
                };
            }

            var builder = new KeyboardBuilder();

            var lastText = texts.Last();

            foreach (var text in texts.SkipLast(1))
            {
                builder.AddButton(GetButton(text));
                builder.AddLine();
            }

            builder.AddButton(GetButton(lastText));

            return builder.Build();
        }
    }
}
