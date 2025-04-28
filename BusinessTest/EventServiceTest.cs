using EventPlanner.Business;
using EventPlanner.Entities.Models;
using EventPlanner.Entities.Models.Dto;
using EventPlanner.Repository;
using Moq;

namespace EventPlanner.BusinessTest
{
    public class EventServiceTests
    {
        private readonly Mock<IEventsRepository> _mockEventRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IParticipantRepository> _mockParticipantRepository;
        private readonly Mock<IPlanGenerator> _mockPlanGenerator;
        private readonly EventService _eventService;

        public EventServiceTests()
        {
            _mockEventRepository = new Mock<IEventsRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockParticipantRepository = new Mock<IParticipantRepository>();
            _mockPlanGenerator = new Mock<IPlanGenerator>();

            _eventService = new EventService(
                _mockEventRepository.Object,
                _mockUserRepository.Object,
                _mockParticipantRepository.Object,
                _mockPlanGenerator.Object
            );
        }

        [Fact]
        public async Task CreateEventAsync_ShouldCreateEvent()
        {
            // Arrange
            var newEvent = new EventCreateDto
            {
                UserId = 1,
                EventName = "Test Event",
                TelegramChatId = 12345
            };
            var user = new User
            {
                Id = 1,
                TelegramId = 12345,
                Username = "TestUser",
                FirstName = "Test",
                LastName = "User",
                Availabilities = new List<UserAvailability>(),
                CreatedEvents = new List<Event>(),
                Participations = new List<Participant>(),
                AssignedTasks = new List<TaskItem>(),
                Votes = new List<Vote>(),
                GeneratedPlans = new List<LLMGeneratedPlan>(),
                UploadedFiles = new List<EventDocument>()
            };
            _mockUserRepository.Setup(repo => repo.GetUserByTelegramIdAsync(newEvent.UserId))
                .ReturnsAsync(user);

            _mockEventRepository.Setup(repo => repo.CreateAsync(It.IsAny<Event>()))
                .ReturnsAsync(new Event
                {
                    Title = newEvent.EventName,
                    Description = string.Empty,
                    Creator = user,
                    Participants = new List<Participant>(),
                    Tasks = new List<TaskItem>(),
                    Files = new List<EventDocument>(),
                    Votes = new List<Vote>(),
                    GeneratedPlans = new List<LLMGeneratedPlan>()
                });

            // Act
            var result = await _eventService.CreateEventAsync(newEvent);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Event", result.Title);
            _mockEventRepository.Verify(repo => repo.CreateAsync(It.IsAny<Event>()), Times.Once);
        }

        [Fact]
        public async Task GetEventByTelegramChatIdAsync_ShouldReturnEvent()
        {
            // Arrange
            var telegramChatId = 12345;
            var @event = new Event

            {
                Id = 1,
                TelegramChatId = telegramChatId,
                Title = string.Empty,
                Description = string.Empty,
                Creator = new User
                {
                    Username = string.Empty,
                    FirstName = string.Empty,
                    LastName = string.Empty,
                    Availabilities = new List<UserAvailability>(),
                    CreatedEvents = new List<Event>(),
                    Participations = new List<Participant>(),
                    AssignedTasks = new List<TaskItem>(),
                    UploadedFiles = new List<EventDocument>(),
                    Votes = new List<Vote>(),
                    GeneratedPlans = new List<LLMGeneratedPlan>()
                },
                Participants = new List<Participant>(),
                Tasks = new List<TaskItem>(),
                Files = new List<EventDocument>(),
                Votes = new List<Vote>(),
                GeneratedPlans = new List<LLMGeneratedPlan>()
            };
            _mockEventRepository.Setup(repo => repo.GetByTelegramChatIdAsync(telegramChatId))
                .ReturnsAsync(@event);

            // Act
            var result = await _eventService.GetEventByTelegramChatIdAsync(telegramChatId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(telegramChatId, result.TelegramChatId);
        }

        [Fact]
        public async Task UpdateEventAsync_ShouldUpdateEvent()
        {
            // Arrange
            var eventId = 1;
            var eventUpdateDto = new EventUpdateDto { Title = "Updated Title" };
            var existingEvent = new Event
            {
                Id = eventId,
                Title = "Old Title",
                Description = string.Empty,
                Creator = new User
                {
                    Username = string.Empty,
                    FirstName = string.Empty,
                    LastName = string.Empty,
                    Availabilities = new List<UserAvailability>(),
                    CreatedEvents = new List<Event>(),
                    Participations = new List<Participant>(),
                    AssignedTasks = new List<TaskItem>(),
                    UploadedFiles = new List<EventDocument>(),
                    Votes = new List<Vote>(),
                    GeneratedPlans = new List<LLMGeneratedPlan>()
                },
                Participants = new List<Participant>(),
                Tasks = new List<TaskItem>(),
                Files = new List<EventDocument>(),
                Votes = new List<Vote>(),
                GeneratedPlans = new List<LLMGeneratedPlan>()
            };

            _mockEventRepository.Setup(repo => repo.GetByIdAsync(eventId))
                .ReturnsAsync(existingEvent);
            _mockEventRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Event>()))
                .ReturnsAsync(new Event
                {
                    Id = eventId,
                    Title = "Updated Title",
                    Description = string.Empty,
                    Creator = new User
                    {
                        Username = string.Empty,
                        FirstName = string.Empty,
                        LastName = string.Empty,
                        Availabilities = new List<UserAvailability>(),
                        CreatedEvents = new List<Event>(),
                        Participations = new List<Participant>(),
                        AssignedTasks = new List<TaskItem>(),
                        UploadedFiles = new List<EventDocument>(),
                        Votes = new List<Vote>(),
                        GeneratedPlans = new List<LLMGeneratedPlan>()
                    },
                    Participants = new List<Participant>(),
                    Tasks = new List<TaskItem>(),
                    Files = new List<EventDocument>(),
                    Votes = new List<Vote>(),
                    GeneratedPlans = new List<LLMGeneratedPlan>()
                });

            // Act
            var result = await _eventService.UpdateEventAsync(eventId, eventUpdateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Title", result.Title);
        }

        [Fact]
        public async Task AddParticipantAsync_ShouldAddParticipant()
        {
            // Arrange
            var eventId = 1;
            var participantId = 2;
            var user = new User
            {
                Id = participantId,
                Username = string.Empty,
                FirstName = string.Empty,
                LastName = string.Empty,
                Availabilities = new List<UserAvailability>(),
                CreatedEvents = new List<Event>(),
                Participations = new List<Participant>(),
                AssignedTasks = new List<TaskItem>(),
                UploadedFiles = new List<EventDocument>(),
                Votes = new List<Vote>(),
                GeneratedPlans = new List<LLMGeneratedPlan>()
            };
            var @event = new Event
            {
                Id = eventId,
                Title = string.Empty,
                Description = string.Empty,
                Creator = new User
                {
                    Username = string.Empty,
                    FirstName = string.Empty,
                    LastName = string.Empty,
                    Availabilities = new List<UserAvailability>(),
                    CreatedEvents = new List<Event>(),
                    Participations = new List<Participant>(),
                    AssignedTasks = new List<TaskItem>(),
                    UploadedFiles = new List<EventDocument>(),
                    Votes = new List<Vote>(),
                    GeneratedPlans = new List<LLMGeneratedPlan>()
                },
                Participants = new List<Participant>(),
                Tasks = new List<TaskItem>(),
                Files = new List<EventDocument>(),
                Votes = new List<Vote>(),
                GeneratedPlans = new List<LLMGeneratedPlan>()
            };

            _mockUserRepository.Setup(repo => repo.GetByIdAsync(participantId))
                .ReturnsAsync(user);
            _mockEventRepository.Setup(repo => repo.GetByIdAsync(eventId))
                .ReturnsAsync(@event);
            _mockParticipantRepository.Setup(repo => repo.GetParticipantsByEventIdAsync(eventId))
                .ReturnsAsync(new List<Participant>());

            // Act
            await _eventService.AddParticipantAsync(eventId, participantId);

            // Assert
            _mockParticipantRepository.Verify(repo => repo.CreateAsync(It.IsAny<Participant>()), Times.Once);
        }

        [Fact]
        public async Task GeneratePlanAsync_ShouldGeneratePlan()
        {
            // Arrange
            var eventId = 1;
            var userId = 2;
            var prompt = "Plan prompt";

            var user = new User
            {
                Id = userId,
                Username = string.Empty,
                FirstName = string.Empty,
                LastName = string.Empty,
                Availabilities = new List<UserAvailability>(),
                CreatedEvents = new List<Event>(),
                Participations = new List<Participant>(),
                AssignedTasks = new List<TaskItem>(),
                UploadedFiles = new List<EventDocument>(),
                Votes = new List<Vote>(),
                GeneratedPlans = new List<LLMGeneratedPlan>()
            };
            var @event = new Event
            {
                Id = eventId,
                Title = string.Empty,
                Description = string.Empty,
                Creator = new User
                {
                    Username = string.Empty,
                    FirstName = string.Empty,
                    LastName = string.Empty,
                    Availabilities = new List<UserAvailability>(),
                    CreatedEvents = new List<Event>(),
                    Participations = new List<Participant>(),
                    AssignedTasks = new List<TaskItem>(),
                    UploadedFiles = new List<EventDocument>(),
                    Votes = new List<Vote>(),
                    GeneratedPlans = new List<LLMGeneratedPlan>()
                },
                Participants = new List<Participant>
                {
                },
                Tasks = new List<TaskItem>(),
                Files = new List<EventDocument>(),
                Votes = new List<Vote>(),
                GeneratedPlans = new List<LLMGeneratedPlan>()
            };


            @event.Participants.Add(new Participant
            {
                UserId = userId,
                EventId = eventId,
                User = user,
                Event = @event
            });

            _mockEventRepository.Setup(repo => repo.GetEventWithDetailsAsync(eventId))
                .ReturnsAsync((Event?)@event);
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId))
                .ReturnsAsync(user);
            _mockPlanGenerator.Setup(generator => generator.GeneratePlanAsync(@event, prompt))
                .ReturnsAsync("Generated Plan");

            _mockEventRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Event>()))
                .ReturnsAsync((Event e) => e);

            // Act
            var result = await _eventService.GeneratePlanAsync(eventId, userId, prompt);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.GeneratedPlans);
            Assert.Equal("Generated Plan", result.GeneratedPlans.First().PlanText);
        }

        [Fact]
        public async Task GeneratePlanAsync_ShouldThrowException_WhenUserNotFound()
        {
            // Arrange
            var eventId = 1;
            var userId = 2;
            var prompt = "Plan prompt";

            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId))
                               .ReturnsAsync((User?)null!);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _eventService.GeneratePlanAsync(eventId, userId, prompt));
        }

        [Fact]
        public async Task GeneratePlanAsync_ShouldThrowException_WhenEventNotFound()
        {
            // Arrange
            var eventId = 1;
            var userId = 2;
            var prompt = "Plan prompt";

            _mockEventRepository.Setup(repo => repo.GetEventWithDetailsAsync(eventId))
                .ReturnsAsync((Event)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _eventService.GeneratePlanAsync(eventId, userId, prompt));
        }

        [Fact]
        public async Task AddParticipantAsync_ShouldNotAddDuplicateParticipant()
        {
            // Arrange
            var eventId = 1;
            var participantId = 2;
            var user = new User
            {
                Id = participantId,
                Username = string.Empty,
                FirstName = string.Empty,
                LastName = string.Empty,
                Availabilities = new List<UserAvailability>(),
                CreatedEvents = new List<Event>(),
                Participations = new List<Participant>(),
                AssignedTasks = new List<TaskItem>(),
                UploadedFiles = new List<EventDocument>(),
                Votes = new List<Vote>(),
                GeneratedPlans = new List<LLMGeneratedPlan>()
            };
            var @event = new Event
            {
                Id = eventId,
                Title = string.Empty,
                Description = string.Empty,
                Creator = new User
                {
                    Username = string.Empty,
                    FirstName = string.Empty,
                    LastName = string.Empty,
                    Availabilities = new List<UserAvailability>(),
                    CreatedEvents = new List<Event>(),
                    Participations = new List<Participant>(),
                    AssignedTasks = new List<TaskItem>(),
                    UploadedFiles = new List<EventDocument>(),
                    Votes = new List<Vote>(),
                    GeneratedPlans = new List<LLMGeneratedPlan>()
                },
                Tasks = new List<TaskItem>(),
                Files = new List<EventDocument>(),
                Votes = new List<Vote>(),
                GeneratedPlans = new List<LLMGeneratedPlan>(),
                Participants = new List<Participant>()
            };

            @event.Participants.Add(new Participant
            {
                UserId = participantId,
                EventId = eventId,
                User = user,
                Event = @event
            });

            _mockUserRepository.Setup(repo => repo.GetByIdAsync(participantId))
                .ReturnsAsync(user);
            _mockEventRepository.Setup(repo => repo.GetByIdAsync(eventId))
                .ReturnsAsync(@event);
            _mockParticipantRepository.Setup(repo => repo.GetParticipantsByEventIdAsync(eventId))
                .ReturnsAsync(@event.Participants);

            // Act
            await _eventService.AddParticipantAsync(eventId, participantId);

            // Assert
            _mockParticipantRepository.Verify(repo => repo.CreateAsync(It.IsAny<Participant>()), Times.Never);
        }

        [Fact]
        public async Task GetEventByTelegramChatIdAsync_ShouldReturnNull_WhenEventNotFound()
        {
            // Arrange
            var telegramChatId = 12345;

            _mockEventRepository.Setup(repo => repo.GetByTelegramChatIdAsync(telegramChatId))
                .ReturnsAsync((Event)null);

            // Act
            var result = await _eventService.GetEventByTelegramChatIdAsync(telegramChatId);

            // Assert
            Assert.Null(result);
        }

    }
}