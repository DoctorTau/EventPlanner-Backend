using EventPlanner.Entities.Models;

namespace EventPlanner.Repository
{
    public interface IEventsRepository : IRepository<Event>
    {
        Task<IEnumerable<Event>> GetByCreatorAsync(int creatorId);

        Task<IEnumerable<Event>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        Task<IEnumerable<Event>> GetByLocationAsync(string location);

        Task<Event> GetEventWithDetailsAsync(int eventId); // Includes Participants & Tasks
    }
}