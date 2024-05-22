using MotoRentalService.Application.MediatR.Response.Auth;

namespace MotoRentalService.Application.MediatR.Response.Common
{
    /// <summary>
    /// Represents the result of a command operation in the application, including creation, update, and deletion actions.
    /// </summary>
    public class CommandResult : ApiResult
    {
        /// <summary>
        /// Gets or sets the ID associated with the command operation (for Create, Update, Delete operations).
        /// </summary>
        public int? Id { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="CommandResult"/> class.
        /// </summary>
        public CommandResult()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandResult"/> class.
        /// </summary>
        /// <param name="success">Indicates whether the command operation was successful.</param>
        /// <param name="message">Provides a message describing the result of the command operation.</param>
        /// <param name="id">The ID associated with the command operation, if applicable.</param>
        public CommandResult(bool success = false, string message = null, int? id = null)
        {
            Success = success;
            Message = message;
            Id = id;
        }
    }
}
