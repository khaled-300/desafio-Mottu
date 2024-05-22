namespace MotoRentalService.Application.Request.Rental
{
    public class CreateRentalRequest
    {
        public int MotorcycleId { get; set; }
        public int UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int RentalPlanId { get; set; }
    }
}
