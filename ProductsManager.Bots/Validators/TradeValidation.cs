using FluentValidation;

namespace ProductsManager.Bots.Validators
{
    public class TradeValidation : AbstractValidator<string>
    {
        public TradeValidation()
        {
            RuleFor(s => s).Must(IsValid).WithMessage("Ошибка валидации");
        }

        private bool IsValid(string value)
        {
            if (value.Where(c => c == ' ').Count() != 1)
            {
                return false;
            }

            var numbers = value.Split(' ');

            int id;
            int count;

            if (!int.TryParse(numbers[0], out id) || !int.TryParse(numbers[1], out count))
            {
                return false;
            }

            if (count < 1)
            {
                return false;
            }

            return true;
        }
    }
}
