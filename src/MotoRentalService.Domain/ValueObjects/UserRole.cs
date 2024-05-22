using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoRentalService.Domain.ValueObjects
{
    /// <summary>
    /// Represents the roles available for user authentication.
    /// </summary>
    public enum UserRole
    {
        /// <summary>
        /// Administrator role.
        /// </summary>
        Admin,

        /// <summary>
        /// Regular user role.
        /// </summary>
        User
    }
}
