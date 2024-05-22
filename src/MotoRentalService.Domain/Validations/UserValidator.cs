using FluentValidation;
using MotoRentalService.Domain.Entities;
using System.Text.RegularExpressions;

namespace MotoRentalService.Domain.Validations
{
    public class UserValidator : AbstractValidator<DeliveryUser>
    {
        public UserValidator()
        {
            RuleFor(p => p.Name).NotEmpty().WithMessage("Name should not be empty");
            RuleFor(p => p.CNPJ).NotEmpty().WithMessage("CNPJ Number should not be empty").Must(BeAValidCNPJ).WithMessage("CNPJ must be exactly 14 digits long without any special characters");
            RuleFor(p => p.LicenseNumber).NotEmpty().WithMessage("Driver's License Number should not be empty");
            RuleFor(p => p.DateOfBirth).NotEmpty().WithMessage("Date of Birth should not be empty");
            RuleFor(p => p.LicenseType).IsInEnum().WithMessage("Invalid Driver's License Type (A, B, or A+B)");
        }
        private bool BeAValidCNPJ(string cnpj)
        {
            if (string.IsNullOrWhiteSpace(cnpj))
                return false;

            var digitsOnly = Regex.Replace(cnpj, @"[^\d]", "");
            return digitsOnly.Length == 14;
        }
    }
}
