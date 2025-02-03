using FluentValidation;

namespace PANDA.Api.Validators
{
    public class NhsNumberValidator : AbstractValidator<string>
    {
        public NhsNumberValidator()
        {
            RuleFor(nhsNumber => nhsNumber)
                .NotEmpty()
                .Length(10)
                .Must(BeValidNhsNumber)
                .WithMessage("Invalid NHS Number.");
        }

        private bool BeValidNhsNumber(string nhsNumber)
        {
            if (nhsNumber.Length != 10 || !nhsNumber.All(char.IsDigit))
            {
                return false;
            }

            int[] weights = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int total = 0;

            for (int i = 0; i < 9; i++)
            {
                total += (nhsNumber[i] - '0') * weights[i];
            }

            int remainder = total % 11;
            int checkDigit = 11 - remainder;

            if (checkDigit == 11)
            {
                checkDigit = 0;
            }
            else if (checkDigit == 10)
            {
                return false;
            }

            return checkDigit == (nhsNumber[9] - '0');
        }
    }
}
