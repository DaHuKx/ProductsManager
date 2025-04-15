using Microsoft.Extensions.Logging;
using ProductsManager.Bots.Interfaces;
using ProductsManager.Bots.Models;
using ProductsManager.Infrastructure.DataBase.Enums;
using System.Net.Http.Headers;
using System.Text;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Enums.StringEnums;
using VkNet.Exception;
using VkNet.Model;

namespace ProductsManager.Bots.Clients
{
    public class VkBot : BotBase, IBot
    {
        private readonly ulong _groupId;
        private readonly ILogger _logger;

        private readonly VkApi _api;
        private readonly Random _random;

        private ulong? _ts;

        public event BotMessageHandler? OnGotMessage;

        public VkBot(ulong groupId, ILogger logger)
        {
            BotType = BotType.Vk;

            _groupId = groupId;
            _random = new Random();
            _api = new VkApi();

            _logger = logger;
        }

        public VkBot(string accessToken, ulong groupId, ILogger logger) : this(groupId, logger)
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
                _logger.LogError("Bot not authorized.");
                return;
            }

            List<MediaAttachment>? attachments = null;
            if (!string.IsNullOrEmpty(message.DocumentPath))
            {
                var server = await _api.Docs.GetMessagesUploadServerAsync(long.Parse(message.UserId), DocMessageType.Doc);

                var response = await UploadFile(server.UploadUrl, message.DocumentPath, Path.GetExtension(message.DocumentPath));

                string title = Path.GetFileName(message.DocumentPath);

                try
                {
                    attachments = new List<MediaAttachment>
                    {
                        _api.Docs.Save(response, title ?? Guid.NewGuid().ToString(), null)
                                 .First()
                                 .Instance
                    };
                }
                catch (Exception ex)
                {
                    message.Message = "Ошибка во время отправки файла на сервер. Попробуй позже. 🚫";
                    _logger.LogError($"UploadFile error: {ex.Message}, User: {message.UserId}");
                }
            }

            await _api.Messages.SendAsync(new MessagesSendParams
            {
                Message = message.Message,
                UserId = long.Parse(message.UserId),
                RandomId = _random.Next(),
                Keyboard = message.KeyboardTexts is null ? default : CreateKeyboard(message.KeyboardTexts),
                Attachments = attachments
            });
        }

        public async Task StartListeningMessages()
        {
            while (true)
            {
                try
                {
                    var response = await GetBotsLongPollHistoryResponseAsync();

                    if (response?.Updates is null || response.Updates.Count == 0)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }

                    foreach (var update in response.Updates.Where(u => u.Type.Value == GroupUpdateType.MessageNew))
                    {
                        CheckUpdate(update);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Long poll getting error - {ex.Message}");
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

        private async Task<BotsLongPollHistoryResponse> GetBotsLongPollHistoryResponseAsync()
        {
            var pollResponse = await _api.Groups.GetLongPollServerAsync(_groupId);

            BotsLongPollHistoryResponse response;

            try
            {
                response = await _api.Groups.GetBotsLongPollHistoryAsync(new BotsLongPollHistoryParams
                {
                    Server = pollResponse.Server,
                    Ts = _ts ?? pollResponse.Ts,
                    Key = pollResponse.Key,
                    Wait = 90
                });

                _ts = response.Ts;

                return response;
            }
            catch (LongPollOutdateException outDateEx)
            {
                _ts = outDateEx.Ts;

                return null;
            }
            catch (Exception ex)
            {
                var type = ex.GetType();

                return null;
            }
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

        private async Task<string> UploadFile(string serverUrl, string file, string fileExtension)
        {
            // Получение массива байтов из файла
            var data = File.ReadAllBytes(file);

            // Создание запроса на загрузку файла на сервер
            using (var client = new HttpClient())
            {
                var requestContent = new MultipartFormDataContent();
                var content = new ByteArrayContent(data);
                content.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
                requestContent.Add(content, "file", $"file.{fileExtension}");

                var response = client.PostAsync(serverUrl, requestContent).Result;
                return Encoding.Default.GetString(await response.Content.ReadAsByteArrayAsync());
            }
        }
    }
}
