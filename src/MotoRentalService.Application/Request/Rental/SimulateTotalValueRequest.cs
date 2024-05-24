using MotoRentalService.Domain.Aggregates;

namespace MotoRentalService.Application.Request.Rental
{
    public class SimulateTotalValueRequest
    {
        [SwaggerExclude]
        public int Id { get; set; }
        public DateTime ReturnDate { get; set; }
    }
}
