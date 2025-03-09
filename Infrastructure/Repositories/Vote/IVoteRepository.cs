using EventPlanner.Entities.Models;

namespace EventPlanner.Repository
{
    public interface IVoteRepository : IRepository<Vote>
    {
        Task<IEnumerable<Vote>> GetVotesByEventIdAsync(int eventId);
        Task<Vote> GetUserVoteAsync(int eventId, int userId);
        Task<string> GetMostPopularVoteOptionAsync(int eventId);
    }
}