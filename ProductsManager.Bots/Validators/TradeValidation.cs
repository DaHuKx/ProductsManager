using FluentValidation;

namespace ProductsManager.Bots.Validators
{
    public class TradeValidation : AbstractValidator<string>
    {
        public TradeValidation()
        {
            RuleFor(s => s).Must(x => x.Count(c => c == ' ') == 1)
                           .Must(IsValid)
                           .WithMessage("Ошибка валидации");
        }

        private bool IsValid(string value)
        {
            var numbers = value.Split(' ');

            if (!int.TryParse(numbers[0], out int id)
             || !int.TryParse(numbers[1], out int count))
            {
                return false;
            }

            return count > 0 && id > 0;
        }
    }
}
