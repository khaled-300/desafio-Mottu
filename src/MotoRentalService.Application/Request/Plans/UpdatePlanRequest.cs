namespace MotoRentalService.Application.Request.Plans
{
    public class UpdatePlanRequest
    {
        public string Name { get; set; }
        public int DurationInDays { get; set; }
        public decimal DailyRate { get; set; }
        public bool IsActive { get; set; }
    }
}
