using ProductsManager.Bots.Models;
using ProductsManager.Infrastructure.DataBase.Enums;

namespace ProductsManager.Bots.Interfaces
{
    public interface IMessageHandler
    {
        public UserPlace Place { get; }
        public Task<BotMessage> ResolveMessage(BotMessage message);
    }
}
