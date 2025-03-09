using EventPlanner.Entities.Models;

namespace EventPlanner.Repository
{
    public interface IUserRepository : IRepository<User>
    {
        public Task<User> GetUserByTelegramIdAsync(int telegramId);
    }

}