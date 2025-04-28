using EventPlanner.Business;
using EventPlanner.Entities.Models;
using EventPlanner.Entities.Models.Dto;
using EventPlanner.Repository;
using Microsoft.VisualStudio.TestPlatform.Common.Utilities;
using Moq;
using Xunit;

namespace EventPlanner.BusinessTest
{
    public class PollServiceTest
    {
        private readonly Mock<IVoteRepository> _mockVoteRepository;
        private readonly Mock<IPollRepository> _mockPollRepository;
        private readonly Mock<IEventsRepository> _mockEventRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IUserAvailabilityRepository> _mockUserAvailabilityRepository;
        private readonly Mock<IParticipantRepository> _mockParticipantRepository;
        private readonly Mock<IChatService> _mockChatService;
        private readonly PollService _pollService;

        public PollServiceTest()
        {
            _mockVoteRepository = new Mock<IVoteRepository>();
            _mockPollRepository = new Mock<IPollRepository>();
            _mockEventRepository = new Mock<IEventsRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockUserAvailabilityRepository = new Mock<IUserAvailabilityRepository>();
            _mockParticipantRepository = new Mock<IParticipantRepository>();
            _mockChatService = new Mock<IChatService>();

            _pollService = new PollService(
                _mockVoteRepository.Object,
                _mockPollRepository.Object,
                _mockEventRepository.Object,
                _mockUserRepository.Object,
                _mockUserAvailabilityRepository.Object,
                _mockParticipantRepository.Object,
                _mockChatService.Object
            );
        }

        [Fact]
        public async Task CreatePollAsync_ShouldCreatePoll()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                TelegramId = 12345,
                FirstName = "Test",
                LastName = "User",
                Username = "testuser",
                CreatedAt = DateTime.UtcNow,
                Availabilities = [],
                CreatedEvents = [],
                Participations = [],
                AssignedTasks = [],
                UploadedFiles = [],
                Votes = [],
                GeneratedPlans = []
            };
            var @event = new Event
            {
                Id = 1,
                Title = "Test Event",
                Description = "Test Description",
                Creator = user,
                Participants = [],
                Tasks = [],
                Files = [],
                Votes = [],
                GeneratedPlans = [],
            };

            var poll = new Poll
            {
                EventId = 1,
                Options = new List<string> { "Option 1", "Option 2" },
                Votes = [],
                Event = @event
            };

            var pollCreateDto = new PollCreateDto(poll)
            {
                EventId = 1,
                Options = poll.Options
            };
            _mockEventRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(@event);
            _mockPollRepository.Setup(repo => repo.CreateAsync(It.IsAny<Poll>())).ReturnsAsync(poll);

            // Act
            var result = await _pollService.CreatePollAsync(pollCreateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.EventId);
            Assert.Equal(2, result.Options.Count);
            _mockPollRepository.Verify(repo => repo.CreateAsync(It.IsAny<Poll>()), Times.Once);
        }

        [Fact]
        public async Task CreateDatePollAsync_ShouldCreateDatePoll()
        {
            // Arrange
            var eventId = 1;
            var user = new User
            {
                Id = 1,
                TelegramId = 12345,
                FirstName = "Test",
                LastName = "User",
                Username = "testuser",
                CreatedAt = DateTime.UtcNow,
                Availabilities = [],
                CreatedEvents = [],
                Participations = [],
                AssignedTasks = [],
                UploadedFiles = [],
                Votes = [],
                GeneratedPlans = []
            };

            var userSecond = new User
            {
                Id = 2,
                TelegramId = 12346,
                FirstName = "Test",
                LastName = "User",
                Username = "testuser",
                CreatedAt = DateTime.UtcNow,
                Availabilities = [],
                CreatedEvents = [],
                Participations = [],
                AssignedTasks = [],
                UploadedFiles = [],
                Votes = [],
                GeneratedPlans = []
            };
            var @event = new Event
            {
                Id = eventId,
                Title = "Test Event",
                Description = "Test Description",
                Creator = user,
                Participants = [],
                Tasks = [],
                Files = [],
                Votes = [],
                GeneratedPlans = [],
            };
            var participants = new List<Participant>
            {
                new Participant { UserId = 1, Event = @event, User = user },
                new Participant { UserId = 2, Event = @event, User = userSecond }
            };
            var userAvailabilities = new List<UserAvailability>
            {
                new UserAvailability { UserId = 1, AvailableDate = DateTime.UtcNow, User = user },
                new UserAvailability { UserId = 1, AvailableDate = DateTime.UtcNow, User = user },
                new UserAvailability { UserId = 2, AvailableDate = DateTime.UtcNow, User = userSecond },
                new UserAvailability { UserId = 2, AvailableDate = DateTime.UtcNow, User = userSecond }
            };

            user.Availabilities = userAvailabilities;
            userSecond.Availabilities = userAvailabilities;
            @event.Participants = participants;
            var createdPoll = new Poll
            {
                Id = 1,
                Options = new List<string>(),
                Event = @event,
                Votes = new List<Vote>(),
                EventId = eventId,
                Status = PollStatus.Pending
            };

            _mockEventRepository.Setup(repo => repo.GetByIdAsync(eventId)).ReturnsAsync(@event);
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(user);
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(2)).ReturnsAsync(userSecond);
            _mockParticipantRepository.Setup(repo => repo.GetParticipantsByEventIdAsync(eventId)).ReturnsAsync(participants);
            _mockUserAvailabilityRepository.Setup(repo => repo.GetByUserIdAsync(It.IsAny<int>())).ReturnsAsync(userAvailabilities);
            _mockPollRepository.Setup(repo => repo.CreateAsync(It.IsAny<Poll>())).ReturnsAsync(createdPoll);
            _mockPollRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(createdPoll);

            // Act
            var result = await _pollService.CreateDatePollAsync(eventId);

            // Assert
            Assert.NotNull(result);
            _mockPollRepository.Verify(repo => repo.CreateAsync(It.IsAny<Poll>()), Times.Once);
        }

        [Fact]
        public async Task CreateVoteAsync_ShouldCreateVote()
        {
            // Arrange
            var voteCreateDto = new VoteCreateDto
            {
                PollId = 1,
                UserId = 12345,
                VoteIndex = 0
            };
            int eventId = 1;
            var user = new User
            {
                Id = 1,
                TelegramId = 12345,
                FirstName = "Test",
                LastName = "User",
                Username = "testuser",
                CreatedAt = DateTime.UtcNow,
                Availabilities = [],
                CreatedEvents = [],
                Participations = [],
                AssignedTasks = [],
                UploadedFiles = [],
                Votes = [],
                GeneratedPlans = []
            };
            var @event = new Event
            {
                Id = eventId,
                Title = "Test Event",
                Description = "Test Description",
                Creator = user,
                Participants = [],
                Tasks = [],
                Files = [],
                Votes = [],
                GeneratedPlans = [],
                LocationPollId = 1
            };
            var poll = new Poll
            {
                Id = 1,
                Status = PollStatus.Open,
                Options = new List<string> { "Option 1", "Option 2" },
                Event = @event,
                EventId = eventId,
                Votes = new List<Vote>(),
            };

            var vote = new Vote
            {
                PollId = 1,
                UserId = 12345,
                VoteOption = "Option 1",
                Poll = poll,
                User = user
            };

            @event.LocationPoll = poll;

            _mockUserRepository.Setup(repo => repo.GetUserByTelegramIdAsync(12345)).ReturnsAsync(user);
            _mockPollRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(poll);
            _mockEventRepository.Setup(repo => repo.GetByIdAsync(eventId)).ReturnsAsync(@event);
            _mockPollRepository.Setup(repo => repo.GetVotesAsync(poll.Id)).ReturnsAsync([vote]);
            _mockVoteRepository.Setup(repo => repo.CreateAsync(It.IsAny<Vote>())).Returns(Task.FromResult(vote));

            // Act
            var result = await _pollService.CreateVoteAsync(voteCreateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.PollId);
            Assert.Equal(1, result.UserId);
            _mockVoteRepository.Verify(repo => repo.CreateAsync(It.IsAny<Vote>()), Times.Once);
        }
    }
}