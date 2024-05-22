using System.ComponentModel.DataAnnotations;

namespace MotoRentalService.Domain.Entities
{
    /// <summary>
    /// Represents a rental plan offered by the motorcycle rental service.
    /// </summary>
    public class RentalPlans : BaseEntity
    {
        /// <summary>
        /// Gets or sets the name of the rental plan.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the duration of the rental plan in days.
        /// </summary>
        [Required]
        public int DurationInDays { get; set; }

        /// <summary>
        /// Gets or sets the daily rate for the rental plan.
        /// </summary>
        [Required]
        [DataType(DataType.Currency)]
        [Range(0, double.MaxValue, ErrorMessage = "Daily rate must be a positive value.")]
        public decimal DailyRate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the rental plan is currently active.
        /// </summary>
        [Required]
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
    }
}
