using FluentValidation;

namespace ProductsManager.Bots.Validators
{
    public class PriceValidation : AbstractValidator<string>
    {
        public PriceValidation()
        {
            RuleFor(x => x).Must(x => x.Count(c => c == ' ') == 1)
                           .Must(IsValid)
                           .WithMessage("Ошибка валидации");
        }

        private bool IsValid(string text)
        {
            var temps = text.Split(' ');

            if (!int.TryParse(temps[0], out int id) ||
                !decimal.TryParse(temps[1], out decimal price))
            {
                return false;
            }

            return id > 0 || price > 0;
        }
    }
}
