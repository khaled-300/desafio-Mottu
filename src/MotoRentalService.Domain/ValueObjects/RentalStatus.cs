namespace MotoRentalService.Domain.ValueObjects
{
    /// <summary>
    /// Defines the possible statuses of a rental contract within the motorcycle rental service.
    /// </summary>
    public enum RentalStatus
    {
        /// <summary>
        /// Indicates that the rental is pending and has not yet started. This status is used for rentals that are scheduled but not yet activated.
        /// </summary>
        Pending,

        /// <summary>
        /// Indicates that the rental is currently active. This status applies to rentals that have begun and are currently in progress.
        /// </summary>
        Active,

        /// <summary>
        /// Indicates that the rental has been completed successfully. This status is used when the rental period has ended and all obligations have been fulfilled.
        /// </summary>
        Completed,

        /// <summary>
        /// Indicates that the rental has been cancelled. This status can apply to rentals that were terminated before they were completed, due to any number of reasons such as breach of contract, customer request, or operational issues.
        /// </summary>
        Cancelled
    }
}
