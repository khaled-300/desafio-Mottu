using MotoRentalService.Domain.ValueObjects;

namespace MotoRentalService.Application.Dtos
{
    /// <summary>
    /// Data transfer object representing details of a rental transaction.
    /// </summary>
    public class RentalDto
    {
        /// <summary>
        /// Gets or sets the identifier for rental.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Identifier of the motorcycle being rented.
        /// </summary>
        public int MotorcycleId { get; set; }

        /// <summary>
        /// User identifier of the delivery person associated with the rental.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Rental plan identifier for this rental.
        /// </summary>
        public int RentalPlanId { get; set; }

        /// <summary>
        /// Start date and time of the rental.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// End date and time of the rental.
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Expected end date and time of the rental.
        /// </summary>
        public DateTime ExpectedEndDate { get; set; }

        /// <summary>
        /// Daily rate charged for the rental.
        /// </summary>
        public decimal DailyRate { get; set; }

        /// <summary>
        /// Total price of the rental.
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// Current status of the rental.
        /// </summary>
        public RentalStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the creation date of the rental record.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the last update date of the rental record.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}
