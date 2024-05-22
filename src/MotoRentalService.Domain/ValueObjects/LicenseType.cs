namespace MotoRentalService.Domain.ValueObjects
{
    /// <summary>
    /// Driver lincense types
    /// </summary>
    public enum LicenseType
    {
        /// <summary>
        /// Default value for the first registration if the value is not provided by the user.
        /// </summary>
        None,
        /// <summary>
        /// Two- or three-wheeled vehicles, regardless of cylinder capacity.
        /// </summary>
        A,
        /// <summary>
        /// Passenger cars, pickup trucks, SUVs (utility vehicles), vans, vans, and certain types of trucks, as long as they respect the weight mentioned above and the maximum number of occupants.
        /// </summary>
        B,
        /// <summary>
        /// Both categories of A and B
        /// </summary>
        AB
    }

}
