using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoRentalService.Domain.Entities
{
    /// <summary>
    /// Base class for all entities in the MotoRentalService domain.
    /// </summary>
    public class BaseEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the entity.
        /// </summary>
        [Key]
        public int Id { get; set; }

        private DateTime _createdAt;
        /// <summary>
        /// Gets the date and time when the entity was created.
        /// </summary>
        [Required]
        [DataType(DataType.DateTime)]
        [Column(TypeName = "timestamp with time zone")]
        public DateTime CreatedAt
        {
            get { return _createdAt; }
            set { _createdAt = value.Kind == DateTimeKind.Utc ? value : DateTime.SpecifyKind(value, DateTimeKind.Utc); }
        }

        private DateTime? _updatedAt;
        /// <summary>
        /// Gets or sets the date and time when the entity was last updated. 
        /// Can be null if not updated.
        /// </summary>
        [DataType(DataType.DateTime)]
        [Column(TypeName = "timestamp with time zone")]
        public DateTime? UpdatedAt
        {
            get { return _updatedAt; }
            set { _updatedAt = value.HasValue ? (value.Value.Kind == DateTimeKind.Utc ? value : DateTime.SpecifyKind(value.Value, DateTimeKind.Utc)) : value; }
        }
    }
}
