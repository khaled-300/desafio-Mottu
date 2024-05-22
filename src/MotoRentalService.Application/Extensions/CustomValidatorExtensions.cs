using FluentValidation;

namespace MotoRentalService.Application.Extensions
{
    public static class CustomValidatorExtensions
    {
        public static IRuleBuilder<T, decimal> MustBeDecimal<T>(this IRuleBuilder<T, decimal> rule)
        {
            return rule.Custom((property, context) =>
            {
                if (property.GetType() != typeof(decimal))
                {
                    context.AddFailure("The value must be a decimal number.");
                }
            });
        }
    }
}
