using ProductsManager.Bots.Models;
using ProductsManager.Infrastructure.DataBase.Enums;

namespace ProductsManager.Bots.Clients
{
    public delegate void BotMessageHandler(BotMessage message);

    public abstract class BotBase
    {
        public BotType BotType { get; set; }
    }
}
