using ProductsManager.Domain;
using ProductsManager.Infrastructure.DataBase.Enums;

namespace ProductsManager.Infrastructure.DataBase.Entities
{
    public class BotUser : BaseDbEntity
    {
        public string NetId { get; set; }
        public UserPlace Place { get; set; }
        public BotType BotType { get; set; }
    }
}
