using MotoRentalService.Domain.Aggregates;

namespace MotoRentalService.Application.Dtos
{
    /// <summary>
    /// Represents a DTO for the rental plan offered by the motorcycle rental service.
    /// </summary>
    public class PlanDto
    {
        /// <summary>
        /// Gets or sets the identifier for the Plan.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the name of the rental plan.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the duration of the rental plan in days.
        /// </summary>
        public int DurationInDays { get; set; }

        /// <summary>
        /// Gets or sets the daily rate for the rental plan.
        /// </summary>
        public decimal DailyRate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the rental plan is currently active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the creation date of the plan record.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the last update date of the plan record.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}
