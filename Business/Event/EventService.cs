using EventPlanner.Entities.Models;
using EventPlanner.Entities.Models.Dto;
using EventPlanner.Repository;

namespace EventPlanner.Business
{
    public class EventService : IEventService
    {
        private readonly IEventsRepository _eventRepository;
        private readonly IUserRepository _userRepository;
        private readonly IParticipantRepository _participantRepository;

        public EventService(IEventsRepository eventRepository,
                            IUserRepository userRepository,
                            IParticipantRepository participantRepository)
        {
            _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _participantRepository = participantRepository ?? throw new ArgumentNullException(nameof(participantRepository));
        }


        public async Task<Event> CreateEventAsync(EventCreateDto newEvent)
        {
            var creator = await _userRepository.GetUserByTelegramIdAsync(newEvent.UserId);
            var createdEvent = new Event
            {
                Title = newEvent.EventName,
                CreatedAt = DateTime.UtcNow,
                TelegramChatId = newEvent.TelegramChatId,
                EventDate = null, // To be decided via voting
                Location = null, // To be voted
                CreatorId = newEvent.UserId, // Since this is a group event, no single creator
                Description = string.Empty,
                Creator = creator,
                Participants = [],
                Tasks = [],
                Files = [],
                Votes = [],
                GeneratedPlans = []
            };

            await _eventRepository.CreateAsync(createdEvent);
            return createdEvent;
        }

        public async Task<Event> GetEventByTelegramChatIdAsync(long telegramChatId)
        {
            return await _eventRepository.GetByTelegramChatIdAsync(telegramChatId);
        }

        public async Task UpdateEventDateAsync(int eventId, DateTime selectedDate)
        {
            var eventToUpdate = await _eventRepository.GetByIdAsync(eventId);
            if (eventToUpdate != null)
            {
                eventToUpdate.EventDate = selectedDate;
                await _eventRepository.UpdateAsync(eventToUpdate);
            }
        }

        public async Task UpdateEventLocationAsync(int eventId, string selectedLocation)
        {
            var eventToUpdate = await _eventRepository.GetByIdAsync(eventId);
            if (eventToUpdate != null)
            {
                eventToUpdate.Location = selectedLocation;
                await _eventRepository.UpdateAsync(eventToUpdate);
            }
        }

        public async Task AddParticipantAsync(int eventId, int participantId)
        {
            var user = await _userRepository.GetByIdAsync(participantId);
            var @event = await _eventRepository.GetByIdAsync(eventId);

            Participant participant = new Participant
            {
                EventId = eventId,
                UserId = participantId,
                User = user,
                Event = @event
            };

            await _participantRepository.CreateAsync(participant);
        }

        public async Task<List<Event>> GetAllUsersEventsAsync(int userId)
        {
            return await _eventRepository.GetAllUsersEventsAsync(userId);
        }
    }
}