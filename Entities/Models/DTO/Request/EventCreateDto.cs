namespace EventPlanner.Entities.Models.Dto
{
    public class EventCreateDto
    {
        public string EventName { get; set; } = string.Empty;
        public long TelegramChatId { get; set; }
        public int UserId { get; set; }
    }
}