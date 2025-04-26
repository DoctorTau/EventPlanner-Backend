
namespace EventPlanner.Entities.Models.Dto
{
    public class EventWithParticipantsDto : EventResponseDto
    {
        public List<UserDto> Participants { get; set; } = new List<UserDto>();
        public EventWithParticipantsDto(Event @event, List<UserDto> participants) : base(@event)
        {
            Participants = participants;
        }
    }
}