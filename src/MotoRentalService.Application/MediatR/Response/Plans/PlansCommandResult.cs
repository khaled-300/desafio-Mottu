using MotoRentalService.Application.Dtos;
using MotoRentalService.Application.MediatR.Response.Common;

namespace MotoRentalService.Application.MediatR.Response.Plans
{
    /// <summary>
    /// Represents the result of a command operation involving rental plans.
    /// </summary>
    public class PlansCommandResult : ApiResult
    {
        /// <summary>
        /// Gets or sets the DTO for the plans.
        /// </summary>
        public PlanDto? Plan { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlansCommandResult"/> class.
        /// </summary>
        public PlansCommandResult()
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="PlansCommandResult"/> class.
        /// </summary>
        /// <param name="plan">The DTO for the plans.</param>
        /// <param name="success">Indicates if the command was successful.</param>
        /// <param name="message">The message associated with the command result.</param>
        public PlansCommandResult(PlanDto? plan = null, bool success = false, string message = null)
        {
            Success = success;
            Message = message;
            Plan = plan;
        }
    }
}
