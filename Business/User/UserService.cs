using EventPlanner.Entities.Models;
using EventPlanner.Entities.Models.Dto;
using EventPlanner.Repository;

namespace EventPlanner.Business
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserAvailabilityRepository _userAvailabilityRepository;

        public UserService(IUserRepository userRepository, IUserAvailabilityRepository userAvailabilityRepository)
        {
            _userRepository = userRepository;
            _userAvailabilityRepository = userAvailabilityRepository;
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<User> GetUserAsync(int id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<User> GetUserByTelegramIdAsync(long telegramId)
        {
            return await _userRepository.GetUserByTelegramIdAsync(telegramId);
        }

        public async Task<User> CreateUserAsync(UserDto newUser)
        {
            User createdUser = new User
            {
                TelegramId = newUser.TelegramId,
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                Username = newUser.Username,
                CreatedAt = DateTime.UtcNow,
                Availabilities = [],
                CreatedEvents = [],
                Participations = [],
                AssignedTasks = [],
                UploadedFiles = [],
                Votes = [],
                GeneratedPlans = []
            };

            return await _userRepository.CreateAsync(createdUser);
        }

        public async Task<User> UpdateUserAsync(User updatedUser)
        {
            return await _userRepository.UpdateAsync(updatedUser);
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await GetUserAsync(id);
            await _userRepository.DeleteAsync(user);
        }

        public async Task<List<UserAvailability>> GetUserAvailabilitiesAsync(int userId)
        {
            var UserAvailabilities = await _userAvailabilityRepository.GetByUserIdAsync(userId);
            return UserAvailabilities;
        }

        public async Task AddUserAvailabilityAsync(int userId, UserAvailabilityDto availability)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            UserAvailability userAvailability = new UserAvailability
            {
                UserId = userId,
                User = user,
                AvailableDate = availability.AvailableDate,
                StartTime = availability.StartTime,
                EndTime = availability.EndTime
            };

            await _userAvailabilityRepository.CreateAsync(userAvailability);
        }

        public async Task DeleteUserAvailabilityAsync(int userId, DateTime dateTime)
        {
            var userAvailability = await _userAvailabilityRepository.GetByUserIdAsync(userId);
            if (userAvailability == null)
                throw new KeyNotFoundException($"User availability with user id {userId} not found");

            var userAvailabilityToDelete = userAvailability.FirstOrDefault(ua => ua.AvailableDate == dateTime);
            if (userAvailabilityToDelete == null || userAvailabilityToDelete.AvailableDate != dateTime)
                throw new KeyNotFoundException($"User availability with date {dateTime} not found");
            await _userAvailabilityRepository.DeleteAsync(userAvailabilityToDelete);
        }
    }
}