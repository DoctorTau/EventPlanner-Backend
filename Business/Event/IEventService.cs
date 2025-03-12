using EventPlanner.Entities.Models;
using EventPlanner.Entities.Models.Dto;

namespace EventPlanner.Services
{
    public interface IEventService
    {
        Task<Event> CreateEventAsync(EventCreateDto newEvent);
        Task<Event> GetEventByTelegramChatIdAsync(long telegramChatId);
        Task UpdateEventDateAsync(int eventId, DateTime selectedDate);
        Task UpdateEventLocationAsync(int eventId, string selectedLocation);
        Task AddParticipantAsync(int eventId, int participantId);
    }
}