namespace MotoRentalService.Application.Dtos
{
    /// <summary>
    /// A Data Transfer Object representing a motorcycle available for rental.
    /// </summary>
    public class MotorcycleDto
    {
        /// <summary>
        /// Gets or sets the identifier for the motorcycle.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the year of the motorcycle.
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Gets or sets the model of the motorcycle.
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// Gets or sets the license plate of the motorcycle.
        /// </summary>
        public string LicensePlate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the motorcycle is currently rented.
        /// </summary>
        public bool IsRented { get; set; }

        /// <summary>
        /// Gets or sets the creation date of the motorcycle record.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the last update date of the motorcycle record.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}
