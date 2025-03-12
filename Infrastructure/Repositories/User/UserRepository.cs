using EventPlanner.Data;
using EventPlanner.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace EventPlanner.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IAppDbContext _dbContext;

        public UserRepository(IAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _dbContext.Users.ToListAsync();
        }

        public async Task<User> GetUserByTelegramIdAsync(long telegramId)
        {
            var user = await _dbContext.Users.FindAsync(telegramId);

            if (user == null)
                throw new KeyNotFoundException($"User with telegram id {telegramId} not found");

            return user;
        }

        public async Task<User> GetByIdAsync(int id)
        {
            var user = await _dbContext.Users.FindAsync(id);

            if (user == null)
                throw new KeyNotFoundException($"User with id {id} not found");

            return user;
        }

        public async Task<User> CreateAsync(User user)
        {
            var result = await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<User> UpdateAsync(User user)
        {
            var result = _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task DeleteAsync(User user)
        {
            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
        }

    }
}