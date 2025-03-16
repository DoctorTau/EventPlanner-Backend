using EventPlanner.Entities.Models;

namespace EventPlanner.Repository
{
    public interface IUserAvailabilityRepository : IRepository<UserAvailability>
    {
        public Task<List<UserAvailability>> GetByUserIdAsync(int userId);
    }
}