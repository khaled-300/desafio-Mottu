using MotoRentalService.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MotoRentalService.Domain.Entities
{
    /// <summary>
    /// Represents a rental transaction within the motorcycle rental service.
    /// </summary>
    public class Rental : BaseEntity
    {
        /// <summary>
        /// Gets or sets the identifier of the motorcycle being rented.
        /// </summary>
        [Required]
        [ForeignKey("Motorcycle")]
        public int MotorcycleId { get; set; }

        /// <summary>
        /// Gets or sets the user identifier of the delivery person associated with the rental.
        /// </summary>
        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the rental plan identifier for this rental.
        /// </summary>
        [Required]
        [ForeignKey("RentalPlans")]
        public int RentalPlanId { get; set; }

        /// <summary>
        /// Gets or sets the start date and time of the rental.
        /// </summary>
        [Required]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date and time of the rental.
        /// </summary>
        [Required]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Gets or sets the expected end date and time of the rental.
        /// </summary>
        [Required]
        public DateTime ExpectedEndDate { get; set; }

        /// <summary>
        /// Gets or sets the daily rate charged for the rental.
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal DailyRate { get; set; }

        /// <summary>
        /// Gets or sets the total price of the rental.
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// Gets or sets the current status of the rental.
        /// </summary>
        [Required]
        public RentalStatus Status { get; set; }

        /// <summary>
        /// Assigns or updates the details of the rental.
        /// </summary>
        public void AssignRentalDetails(int deliveryUserId, int motorcycleId, int rentalPlanId, decimal dailyRate, DateTime expectedEndDate, RentalStatus status)
        {
            UserId = deliveryUserId;
            MotorcycleId = motorcycleId;
            RentalPlanId = rentalPlanId;
            DailyRate = dailyRate;
            ExpectedEndDate = expectedEndDate;
            Status = status;
        }

        // Navigation properties
        public virtual Motorcycle Motorcycle { get; set; }
        public virtual DeliveryUser DeliveryPerson { get; set; }
        public virtual RentalPlans RentalPlan { get; set; }
        public virtual List<RentalStatusHistory> StatusHistories { get; set; } = new List<RentalStatusHistory>();
    }
}
