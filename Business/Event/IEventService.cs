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
        Task<Event> UpdateEventAsync(int eventId, EventUpdateDto eventUpdateDto);
        Task UpdateEventDateAsync(int eventId, DateTime selectedDate);
        Task UpdateEventLocationAsync(int eventId, string selectedLocation);
        Task AddParticipantAsync(int eventId, int participantId);
        Task<Event> GeneratePlanAsync(int eventId, int userId, string prompt);
        Task<Event> ModifyPlanAsync(int eventId, int userId, string planToModify, string prompt);
        Task<List<Event>> GetAllUsersEventsAsync(int userId);
    }
}