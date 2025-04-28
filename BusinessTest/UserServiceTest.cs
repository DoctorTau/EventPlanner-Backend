using EventPlanner.Business;
using EventPlanner.Entities.Models;
using EventPlanner.Entities.Models.Dto;
using EventPlanner.Repository;
using Moq;
using Xunit;

namespace EventPlanner.BusinessTest
{
    public class UserServiceTest
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IUserAvailabilityRepository> _mockUserAvailabilityRepository;
        private readonly UserService _userService;

        public UserServiceTest()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockUserAvailabilityRepository = new Mock<IUserAvailabilityRepository>();
            _userService = new UserService(_mockUserRepository.Object, _mockUserAvailabilityRepository.Object);
        }

        [Fact]
        public async Task GetUsersAsync_ShouldReturnAllUsers()
        {
            // Arrange
            var users = new List<User>
            {
                new User{
                    Id = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Username = "johndoe",
                    Availabilities = [],
                    CreatedEvents = [],
                    Participations = [],
                    AssignedTasks = [],
                    UploadedFiles = [],
                    Votes = [],
                    GeneratedPlans = []
                },
                new User
                {
                    Id = 2,
                    FirstName = "Jane",
                    LastName = "Smith",
                    Username = "janesmith",
                    Availabilities = [],
                    CreatedEvents = [],
                    Participations = [],
                    AssignedTasks = [],
                    UploadedFiles = [],
                    Votes = [],
                    GeneratedPlans = []
                }
            };
            _mockUserRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(users);

            // Act
            var result = await _userService.GetUsersAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetUserAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Username = "johndoe",
                Availabilities = [],
                CreatedEvents = [],
                Participations = [],
                AssignedTasks = [],
                UploadedFiles = [],
                Votes = [],
                GeneratedPlans = []
            };
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(user);

            // Act
            var result = await _userService.GetUserAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("John", result.FirstName);
        }

        [Fact]
        public async Task GetUserAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((User)null);

            // Act
            var result = await _userService.GetUserAsync(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserByTelegramIdAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                TelegramId = 12345,
                FirstName = "John",
                LastName = "Doe",
                Username = "johndoe",
                Availabilities = [],
                CreatedEvents = [],
                Participations = [],
                AssignedTasks = [],
                UploadedFiles = [],
                Votes = [],
                GeneratedPlans = [],
            };
            _mockUserRepository.Setup(repo => repo.GetUserByTelegramIdAsync(12345)).ReturnsAsync(user);

            // Act
            var result = await _userService.GetUserByTelegramIdAsync(12345);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(12345, result.TelegramId);
        }

        [Fact]
        public async Task GetUserByTelegramIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            _mockUserRepository.Setup(repo => repo.GetUserByTelegramIdAsync(12345)).ReturnsAsync((User)null);

            // Act
            var result = await _userService.GetUserByTelegramIdAsync(12345);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateUserAsync_ShouldCreateUser()
        {
            // Arrange
            var newUser = new UserDto
            {
                TelegramId = 12345,
                FirstName = "John",
                LastName = "Doe",
                Username = "johndoe"
            };
            var createdUser = new User
            {
                Id = 1,
                TelegramId = 12345,
                FirstName = "John",
                LastName = "Doe",
                Username = "johndoe",
                CreatedAt = DateTime.UtcNow,
                Availabilities = [],
                CreatedEvents = [],
                Participations = [],
                AssignedTasks = [],
                UploadedFiles = [],
                Votes = [],
                GeneratedPlans = [],
            };
            _mockUserRepository.Setup(repo => repo.CreateAsync(It.IsAny<User>())).ReturnsAsync(createdUser);

            // Act
            var result = await _userService.CreateUserAsync(newUser);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("John", result.FirstName);
            _mockUserRepository.Verify(repo => repo.CreateAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldUpdateUser()
        {
            // Arrange
            var updatedUser = new User
            {
                Id = 1,
                FirstName = "Updated",
                LastName = "User",
                Username = "updateduser",
                Availabilities = [],
                CreatedEvents = [],
                Participations = [],
                AssignedTasks = [],
                UploadedFiles = [],
                Votes = [],
                GeneratedPlans = [],
            };
            _mockUserRepository.Setup(repo => repo.UpdateAsync(updatedUser)).ReturnsAsync(updatedUser);

            // Act
            var result = await _userService.UpdateUserAsync(updatedUser);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated", result.FirstName);
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldDeleteUser()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Username = "johndoe",
                Availabilities = [],
                CreatedEvents = [],
                Participations = [],
                AssignedTasks = [],
                UploadedFiles = [],
                Votes = [],
                GeneratedPlans = [],
            };
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(user);
            _mockUserRepository.Setup(repo => repo.DeleteAsync(user)).Returns(Task.CompletedTask);

            // Act
            await _userService.DeleteUserAsync(1);

            // Assert
            _mockUserRepository.Verify(repo => repo.DeleteAsync(user), Times.Once);
        }

        [Fact]
        public async Task GetUserAvailabilitiesAsync_ShouldReturnUserAvailabilities()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Username = "johndoe",
                Availabilities = [],
                CreatedEvents = [],
                Participations = [],
                AssignedTasks = [],
                UploadedFiles = [],
                Votes = [],
                GeneratedPlans = [],
            };
            var availabilities = new List<UserAvailability>
            {
                new UserAvailability { UserId = 1, AvailableDate = DateTime.UtcNow, User = user },
                new UserAvailability { UserId = 1, AvailableDate = DateTime.UtcNow.AddDays(1), User = user }
            };
            _mockUserAvailabilityRepository.Setup(repo => repo.GetByUserIdAsync(1)).ReturnsAsync(availabilities);

            // Act
            var result = await _userService.GetUserAvailabilitiesAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task AddUserAvailabilityAsync_ShouldAddAvailability()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Username = "johndoe",
                Availabilities = [],
                CreatedEvents = [],
                Participations = [],
                AssignedTasks = [],
                UploadedFiles = [],
                Votes = [],
                GeneratedPlans = [],
            };
            var availabilityDto = new UserAvailabilityDto
            {
                AvailableDate = DateTime.UtcNow,
                StartTime = TimeSpan.FromHours(9),
                EndTime = TimeSpan.FromHours(17)
            };
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(user);
            _mockUserAvailabilityRepository.Setup(repo => repo.CreateAsync(It.IsAny<UserAvailability>()))
                .ReturnsAsync(new UserAvailability
                {
                    UserId = 1,
                    AvailableDate = availabilityDto.AvailableDate,
                    StartTime = availabilityDto.StartTime,
                    EndTime = availabilityDto.EndTime,
                    User = user
                });

            // Act
            await _userService.AddUserAvailabilityAsync(1, availabilityDto);

            // Assert
            _mockUserAvailabilityRepository.Verify(repo => repo.CreateAsync(It.IsAny<UserAvailability>()), Times.Once);
        }

        [Fact]
        public async Task DeleteUserAvailabilityAsync_ShouldDeleteAvailability()
        {
            var user = new User
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Username = "johndoe",
                Availabilities = [],
                CreatedEvents = [],
                Participations = [],
                AssignedTasks = [],
                UploadedFiles = [],
                Votes = [],
                GeneratedPlans = [],
            };
            var date = DateTime.UtcNow;
            // Arrange
            var availabilities = new List<UserAvailability>
            {
                new UserAvailability { UserId = 1, AvailableDate = date, StartTime = TimeSpan.FromHours(9), EndTime = TimeSpan.FromHours(17), User =  user},
            };
            _mockUserAvailabilityRepository.Setup(repo => repo.GetByUserIdAsync(1)).ReturnsAsync(availabilities);
            _mockUserAvailabilityRepository.Setup(repo => repo.DeleteAsync(It.IsAny<UserAvailability>())).Returns(Task.CompletedTask);

            // Act
            await _userService.DeleteUserAvailabilityAsync(1, date);

            // Assert
            _mockUserAvailabilityRepository.Verify(repo => repo.DeleteAsync(It.IsAny<UserAvailability>()), Times.Once);
        }
    }
}