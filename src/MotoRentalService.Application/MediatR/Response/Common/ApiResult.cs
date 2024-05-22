namespace MotoRentalService.Application.MediatR.Response.Common
{
    /// <summary>
    /// Base class for all command results.
    /// </summary>
    public abstract class ApiResult
    {
        /// <summary>
        /// Gets or sets a value indicating whether the operation was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets a message describing the result of the operation.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets the list of errors that occurred during the operation.
        /// </summary>
        public List<string> Errors { get; set; } = new List<string>();

        public void AddError(string error)
        {
            Success = false;
            Errors.Add(error);
        }

        public void SetSuccess(string message = "Operation completed successfully.")
        {
            Success = true;
            Message = message;
        }

        /// <summary>
        /// Clears the list of errors.
        /// </summary>
        public void ResetErrors() => Errors.Clear();
    }
}
