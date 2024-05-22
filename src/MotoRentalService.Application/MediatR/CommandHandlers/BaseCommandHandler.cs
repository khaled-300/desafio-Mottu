using FluentValidation;
using FluentValidation.Results;
using MediatR;
using MotoRentalService.Application.MediatR.Response.Common;

namespace MotoRentalService.Application.MediatR.CommandHandlers
{
    public abstract class BaseCommandHandler<TCommand, TResult> : IRequestHandler<TCommand, TResult>
    where TCommand : IRequest<TResult>
    where TResult : ApiResult, new()
    {
        private readonly IValidator<TCommand> _validator;

        public BaseCommandHandler(IValidator<TCommand> validator)
        {
            _validator = validator;
        }

        protected TResult HandleValidationErrors(ValidationResult validationResult)
        {
            var result = new TResult
            {
                Success = validationResult.IsValid,
                Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList()
            };
            return result;
        }

        protected TResult HandleException(Exception ex)
        {
            var result = new TResult
            {
                Success = false
            };
            result.Errors.Add(ex.Message);
            return result;
        }

        public abstract Task<TResult> Handle(TCommand request, CancellationToken cancellationToken);
    }

}
