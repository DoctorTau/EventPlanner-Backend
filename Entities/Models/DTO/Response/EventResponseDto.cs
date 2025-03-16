namespace EventPlanner.Entities.Models.Dto
{
    public class EventResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public long TelegramChatId { get; set; }
        public string Description { get; set; } = string.Empty;

        public DateTime? EventDate { get; set; }
        public string? Location { get; set; } = null;
    }
}