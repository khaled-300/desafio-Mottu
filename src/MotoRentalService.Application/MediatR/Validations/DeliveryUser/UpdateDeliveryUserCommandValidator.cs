using FluentValidation;
using MotoRentalService.Application.MediatR.Commands.User;

namespace MotoRentalService.Application.MediatR.Validations.DeliveryUser
{
    public class UpdateDeliveryUserCommandValidator : AbstractValidator<UpdateDeliveryUserCommand>
    {
        public UpdateDeliveryUserCommandValidator()
        {
            RuleFor(m => m.Id).NotEmpty().WithName("Id");
            RuleFor(p => p.LicenseNumber).NotEmpty().WithName("License Number").WithMessage("Driver's License Number should not be empty");
            RuleFor(m => m.LicenseNumber)
                .MaximumLength(11)
                .When(x => !string.IsNullOrEmpty(x.LicenseNumber))
                .Matches(@"^\d+$").WithMessage("Driver's License Number should only contain numbers.");

            RuleFor(m => m.LicenseType)
                .NotEmpty().WithName("Driver's License Type");

            RuleFor(m => m.LicenseImage)
                .NotNull().WithName("Driver's License Image")
                .Custom((image, context) =>
                {
                    if (image != null && !_allowedContentTypes.Contains(image.ContentType))
                    {
                        context.AddFailure("License Image", "Only image files (PNG, BMP) are allowed.");
                    }
                });
        }
        private readonly string[] _allowedContentTypes = new[] { "image/png", "image/bmp" };
    }
}
