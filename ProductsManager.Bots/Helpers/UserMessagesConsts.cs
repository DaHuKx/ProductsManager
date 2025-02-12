using ProductsManager.Infrastructure.DataBase.Enums;

namespace ProductsManager.Bots.Helpers
{
    public static class UserMessagesConsts
    {
        private static readonly Dictionary<UserPlace, List<string>> _expectedMessages;

        public const string Start = "Начать";

        public const string Back = "Назад";

        public const string GetProducts = "Просмотр товаров";

        public const string GetTrades = "Просмотр оборота товара";

        public const string AddProduct = "Добавить товар";

        public const string AddImport = "Добавить закупку";

        public const string AddExport = "Добавить продажу";

        static UserMessagesConsts()
        {
            _expectedMessages = new Dictionary<UserPlace, List<string>>
            {
                [UserPlace.Start] = new List<string> { Start },
                [UserPlace.Menu] = new List<string>
                {
                    GetProducts,
                    GetTrades,
                    AddProduct,
                    AddImport,
                    AddExport
                },
                [UserPlace.AddProduct] = new List<string> { Back }
            };
        }

        public static List<string>? GetExpectedMessages(UserPlace place)
        {
            if (place == UserPlace.AddProduct ||
                place == UserPlace.AddImport ||
               !_expectedMessages.ContainsKey(place))
            {
                return null;
            }

            return _expectedMessages[place];
        }

        public static List<string> GetBackMessage()
        {
            return new List<string> { Back };
        }
    }
}
