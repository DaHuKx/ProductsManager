using ProductsManager.Bots.Clients;
using ProductsManager.Bots.Models;

namespace ProductsManager.Bots.Interfaces
{
    public interface IBot
    {
        public Task SendMessageAsync(BotMessage message);
        public Task StartListeningMessages();
        public Task Initialize(string accessToken);

        public event BotMessageHandler? OnGotMessage;
    }
}
