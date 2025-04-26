namespace EventPlanner.Entities.Models.Dto
{
    public class EventResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public long TelegramChatId { get; set; }
        public string Description { get; set; } = string.Empty;
        public GroupEventType EventType { get; set; }

        public DateTime? EventDate { get; set; }
        public string? Location { get; set; } = null;

        public EventResponseDto(Event @event)
        {
            Id = @event.Id;
            Title = @event.Title;
            TelegramChatId = @event.TelegramChatId;
            Description = @event.Description;
            EventType = @event.EventType;
            EventDate = @event.EventDate;
            Location = @event.Location;
        }
    }
}