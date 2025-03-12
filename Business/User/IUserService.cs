using EventPlanner.Entities.Models;
using EventPlanner.Entities.Models.Dto;

namespace EventPlanner.Business
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetUsersAsync();
        Task<User> GetUserAsync(int id);
        Task<User> GetUserByTelegramIdAsync(long telegramId);
        Task<User> CreateUserAsync(UserCreateDto newUser);
        Task<User> UpdateUserAsync(User updatedUser);
        Task DeleteUserAsync(int id);

        Task<List<UserAvailability>> GetUserAvailabilitiesAsync(int userId);
        Task AddUserAvailabilityAsync(int userId, UserAvailabilityDto availability);
    }
}