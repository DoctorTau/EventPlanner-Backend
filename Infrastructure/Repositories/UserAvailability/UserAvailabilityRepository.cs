using EventPlanner.Data;
using EventPlanner.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace EventPlanner.Repository
{
    public class UserAvailabilityRepository : IRepository<UserAvailability>
    {
        private readonly IAppDbContext _dbContext;

        public UserAvailabilityRepository(IAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<UserAvailability>> GetAllAsync()
        {
            return await _dbContext.UserAvailabilities.ToListAsync();
        }

        public async Task<UserAvailability> GetByUserIdAsync(int userId)
        {
            var userAvailability = await _dbContext.UserAvailabilities.FindAsync(userId);

            if (userAvailability == null)
                throw new KeyNotFoundException($"User availability with user id {userId} not found");

            return userAvailability;
        }

        public async Task<UserAvailability> GetByIdAsync(int id)
        {
            var userAvailability = await _dbContext.UserAvailabilities.FindAsync(id);

            if (userAvailability == null)
                throw new KeyNotFoundException($"User availability with id {id} not found");

            return userAvailability;
        }

        public async Task<UserAvailability> CreateAsync(UserAvailability userAvailability)
        {
            var result = await _dbContext.UserAvailabilities.AddAsync(userAvailability);
            await _dbContext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<UserAvailability> UpdateAsync(UserAvailability userAvailability)
        {
            var result = _dbContext.UserAvailabilities.Update(userAvailability);
            await _dbContext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task DeleteAsync(UserAvailability userAvailability)
        {
            _dbContext.UserAvailabilities.Remove(userAvailability);
            await _dbContext.SaveChangesAsync();
        }
    }
}