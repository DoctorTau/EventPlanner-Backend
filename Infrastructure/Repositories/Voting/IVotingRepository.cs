using EventPlanner.Entities.Models;

namespace EventPlanner.Repository
{
    public interface IVotingRepository : IRepository<Voting>
    {
        Task<List<Vote>> GetVotesAsync(int eventId);
    }
}