using EventPlanner.Entities.Models;
using EventPlanner.Entities.Models.Dto;

namespace EventPlanner.Business
{
    public interface IEventService
    {
        Task<Event> CreateEventAsync(EventCreateDto newEvent);
        Task<Event> GetEventWithParticipantsAsync(int eventId);
        Task<Event> GetEventWithAllDetailsAsync(int eventId);
        Task<Event> GetEventByTelegramChatIdAsync(long telegramChatId);
        Task UpdateEventDateAsync(int eventId, DateTime selectedDate);
        Task UpdateEventLocationAsync(int eventId, string selectedLocation);
        Task AddParticipantAsync(int eventId, int participantId);
        Task<Event> GeneratePlanAsync(int eventId, int userId, string prompt);
        Task<List<Event>> GetAllUsersEventsAsync(int userId);
    }
}