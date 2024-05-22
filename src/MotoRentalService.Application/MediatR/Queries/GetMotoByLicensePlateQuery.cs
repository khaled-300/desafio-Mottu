using MediatR;
using MotoRentalService.Application.Dtos;

namespace MotoRentalService.Application.MediatR.Queries
{
    public class GetMotoByLicensePlateQuery : IRequest<MotorcycleDto?>
    {
        public string LicensePlate { get; }

        public GetMotoByLicensePlateQuery(string licensePlate)
        {
            LicensePlate = licensePlate;
        }
    }
}
