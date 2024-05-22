using System.ComponentModel.DataAnnotations;

namespace MotoRentalService.Domain.Entities
{
    /// <summary>
    /// Represents a motorcycle available for rental.
    /// </summary>
    public class Motorcycle : BaseEntity
    {
        /// <summary>
        /// Gets or sets the year of the motorcycle.
        /// </summary>
        [Required]
        public int Year { get; set; }

        /// <summary>
        /// Gets or sets the model of the motorcycle.
        /// </summary>
        [Required]
        public string Model { get; set; }

        /// <summary>
        /// Gets or sets the license plate of the motorcycle.
        /// </summary>
        [Required]
        [MaxLength(10)]
        public string LicensePlate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the motorcycle is currently rented.
        /// </summary>
        [Required]
        public bool IsRented { get; set; }

        /// <summary>
        /// Marks the motorcycle as rented.
        /// </summary>
        public void MarkAsRented()
        {
            IsRented = true;
        }
    }
}
