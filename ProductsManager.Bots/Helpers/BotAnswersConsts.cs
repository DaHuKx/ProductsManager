using ProductsManager.Infrastructure.DataBase.Enums;
using System.Text;

namespace ProductsManager.Bots.Helpers
{
    public static class BotAnswersConsts
    {
        private static readonly Dictionary<UserPlace, string> _placeMessages;

        private static readonly Random _random;
        private static readonly List<string> _errorMessages;

        public const string Start = "Жми 'Начать' для входа в меню 🆓";

        public const string Menu = "Выбирай, что хочешь сделать? 👌";

        public const string NoProducts = "Не добавлено ни одного продукта 😒";

        public const string AddProduct = "Напиши название продукта, если хочешь добавить несколько продуктов, можно написать в несколько строк.\n" +
                                         "Примеры:\n\n" +
                                         "Xiaomi Smart Band 8\n" +
                                         "Носки шерстяные\n\n" +
                                         "--------------------\n\n" +
                                         "Xiaomi Smart Band 8";

        public const string AddImport = "Пиши закупки в формате:\n" +
                                        "(Id товара) (Количество)\n\n" +
                                        "Примеры:\n" +
                                        "1 10\n" +
                                        "--------------------\n" +
                                        "1 10\n" +
                                        "2 15";

        public const string AddExport = "Пиши продажи в формате:\n" +
                                        "(Id товара) (Количество)\n\n" +
                                        "Примеры:\n" +
                                        "1 10\n" +
                                        "--------------------\n" +
                                        "1 10\n" +
                                        "2 15";

        static BotAnswersConsts()
        {
            _random = new Random();

            _placeMessages = new Dictionary<UserPlace, string>
            {
                [UserPlace.Start] = Start,
                [UserPlace.Menu] = Menu,
                [UserPlace.AddProduct] = AddProduct,
            };

            _errorMessages = new List<string>
            {
                "Ой, а такого ответа я не ожидал 😳",
                "Думаешь, сможешь меня сломать? Не получится 😑",
                "На это сообщение у меня нет ответа 😟",
                "Даже не знаю, что и ответить 🤔"
            };
        }

        public static string GetMessageForPlace(UserPlace place)
        {
            return _placeMessages[place];
        }

        public static string CreateErrorMessage(List<string> expectedMessages)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine($"{_errorMessages[_random.Next(_errorMessages.Count)]}\n");
            builder.AppendLine($"Ожидаемые сообщения:");

            foreach (var message in expectedMessages)
            {
                builder.AppendLine(message);
            }

            return builder.ToString();
        }
    }
}
