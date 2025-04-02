using EventPlanner.Entities.Models;

namespace EventPlanner.Repository
{
    public interface IPollRepository : IRepository<Poll>
    {
        Task<List<Vote>> GetVotesAsync(int eventId);
    }
}