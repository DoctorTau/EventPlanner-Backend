using EventPlanner.Entities.Models;

namespace EventPlanner.Repository
{
    public interface IUserAvailabilityRepository : IRepository<UserAvailability>
    {
        public Task<IEnumerable<UserAvailability>> GetByUserIdAsync(int userId);
    }
}