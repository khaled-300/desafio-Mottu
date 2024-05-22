using MotoRentalService.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MotoRentalService.Domain.Entities
{
    /// <summary>
    /// Represents the history of changes in rental status for a rental.
    /// </summary>
    public class RentalStatusHistory
    {
        /// <summary>
        /// Gets or sets the unique identifier for the rental status history entry.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the rental identifier to which this status change history belongs.
        /// </summary>
        [Required]
        [ForeignKey("Rental")]
        public int RentalId { get; set; }

        /// <summary>
        /// Gets or sets the status of the rental at the time of this record.
        /// </summary>
        [Required]
        public RentalStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the status change occurred.
        /// </summary>
        [Required]
        public DateTime StatusChangedDate { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the user authentication was created.
        /// </summary>
        [Required]
        [DataType(DataType.DateTime)]
        [Column(TypeName = "timestamp with time zone")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Navigation property back to the associated Rental.
        /// </summary>
        public Rental Rental { get; set; }
    }
}
