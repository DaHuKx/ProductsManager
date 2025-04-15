using ProductsManager.Bots.Models;
using ProductsManager.Infrastructure.DataBase.Enums;

namespace ProductsManager.Bots.Helpers
{
    public static class UserMessagesConsts
    {
        private static readonly Dictionary<UserPlace, List<string>> _expectedMessages;

        public const string Start = "Начать";

        public const string Back = "Назад";

        public const string AddProduct = "Добавить товар";

        public const string AddImport = "Добавить закупку";

        public const string AddExport = "Добавить продажу";

        public const string SetImport = "Установить цену закупки";

        public const string SetExport = "Установить цену продажи";

        public const string Reports = "Отчёты";

        public const string StorageReport = "Состояние склада";

        public const string TimestampTrades = "Отчёт по продажам за период";

        public const string ProductTrades = "Отчёт по закупкам/продажам товара";

        static UserMessagesConsts()
        {
            _expectedMessages = new Dictionary<UserPlace, List<string>>
            {
                [UserPlace.Start] = new List<string> { Start },
                [UserPlace.Menu] = new List<string>
                {
                    Reports,
                    AddProduct,
                    AddImport,
                    AddExport,
                    SetImport,
                    SetExport
                },
                [UserPlace.AddProduct] = new List<string> { Back },
                [UserPlace.AddImport] = new List<string> { Back },
                [UserPlace.AddExport] = new List<string> { Back },
                [UserPlace.SetImport] = new List<string> { Back },
                [UserPlace.SetExport] = new List<string> { Back },
                [UserPlace.Reports] = new List<string>
                {
                    StorageReport,
                    //TimestampTrades,
                    //ProductTrades,
                    Back
                }
            };
        }

        public static List<string>? GetExpectedMessages(UserPlace place)
        {
            return _expectedMessages[place];
        }

        public static List<string> GetBackMessage()
        {
            return new List<string> { Back };
        }

        public static bool IsBackMessage(BotMessage message, out BotMessage? answer)
        {
            if (message.Message == UserMessagesConsts.Back)
            {
                answer = new BotMessage
                {
                    BotType = message.BotType,
                    KeyboardTexts = UserMessagesConsts.GetExpectedMessages(UserPlace.Menu),
                    Message = BotAnswersConsts.Menu,
                    NewUserPlace = UserPlace.Menu,
                    UserId = message.UserId
                };

                return true;
            }

            answer = null;

            return false;
        }

        public static bool IsPlaceWithWriting(UserPlace place)
        {
            switch (place)
            {
                case UserPlace.AddProduct:
                case UserPlace.AddImport:
                case UserPlace.AddExport:
                case UserPlace.SetExport:
                case UserPlace.SetImport:
                    return true;
                default:
                    return false;
            }
        }
    }
}
