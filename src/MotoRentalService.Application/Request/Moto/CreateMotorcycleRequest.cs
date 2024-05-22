namespace MotoRentalService.Application.Request.Moto
{
    public class CreateMotorcycleRequest
    {
        public int Year { get; set; }
        public string Model { get; set; }
        public string LicensePlate { get; set; }
    }
}
