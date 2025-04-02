using EventPlanner.Entities.Models;

namespace EventPlanner.Repository
{
    public interface IVoteRepository : IRepository<Vote>
    {
        Task<IEnumerable<Vote>> GetVotesByPollAsync(int pollId);
        Task<Vote> GetUserVoteAsync(int votingId, int userId);
        Task<string> GetMostPopularVoteOptionAsync(int votingId);
    }
}