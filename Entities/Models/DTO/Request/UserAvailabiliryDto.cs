namespace EventPlanner.Entities.Models.Dto
{
    public class UserAvailabilityDto
    {
        public required DateTime AvailableDate { get; set; }

        public required TimeSpan StartTime { get; set; }

        public required TimeSpan EndTime { get; set; }
    }
}