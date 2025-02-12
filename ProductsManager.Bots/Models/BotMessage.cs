using ProductsManager.Infrastructure.DataBase.Enums;

namespace ProductsManager.Bots.Models
{
    public class BotMessage
    {
        public BotType BotType { get; set; }
        public string UserId { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string>? KeyboardTexts { get; set; }
        public UserPlace? NewUserPlace { get; set; } = null;
    }
}
