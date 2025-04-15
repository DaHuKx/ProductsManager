using ProductsManager.Bots.Models;
using ProductsManager.Infrastructure.DataBase.Entities;

namespace ProductsManager.Bots.Interfaces
{
    public interface IBotMessageResolver
    {
        public Task<BotMessage> ProcessMessage(BotUser user, BotMessage message);
    }
}
