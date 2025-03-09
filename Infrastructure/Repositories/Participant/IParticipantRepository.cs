using EventPlanner.Entities.Models;

namespace EventPlanner.Repository
{
    public interface IParticipantRepository : IRepository<Participant>
    {
        Task<IEnumerable<Participant>> GetParticipantsByEventIdAsync(int eventId);
        Task UpdateParticipantStatusAsync(int eventId, int userId, ParticipantStatus status);
    }
}
