using MotoRentalService.Application.MediatR.Response.Common;

namespace MotoRentalService.Application.MediatR.Response.Rental
{
    public class SimulateTotalValueResult: ApiResult
    {
        /// <summary>
        /// Total value with fee 
        /// </summary>
        public decimal? TotalValue { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimulateTotalValueResult"/> class.
        /// </summary>
        public SimulateTotalValueResult()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RentalCommandResult"/> class.
        /// </summary>
        /// <param name="value">The Value of the calculated rental contact.</param>
        /// <param name="success">A value indicating whether the rental operation was successful.</param>
        /// <param name="message">A message describing the result of the rental operation.</param>
        public SimulateTotalValueResult(decimal? value = null, bool success = false, string message = null)
        {
            Success = success;
            Message = message;
            TotalValue = value;
        }
    }
}
