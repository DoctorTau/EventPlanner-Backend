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
        private readonly IPlanGenerator _planGenerator;

        public EventService(IEventsRepository eventRepository,
                            IUserRepository userRepository,
                            IParticipantRepository participantRepository,
                            IPlanGenerator planGenerator)
        {
            _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _participantRepository = participantRepository ?? throw new ArgumentNullException(nameof(participantRepository));
            _planGenerator = planGenerator ?? throw new ArgumentNullException(nameof(planGenerator));
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

            var existingParticipant = await _participantRepository.GetParticipantsByEventIdAsync(eventId);
            if (existingParticipant.Any(p => p.UserId == participantId))
                return;

            await _participantRepository.CreateAsync(participant);
        }

        public async Task<List<Event>> GetAllUsersEventsAsync(int userId)
        {
            return await _eventRepository.GetAllUsersEventsAsync(userId);
        }

        public Task<Event> GetEventWithParticipantsAsync(int eventId)
        {
            return _eventRepository.GetEventWithDetailsAsync(eventId);
        }

        public async Task<Event> GeneratePlanAsync(int eventId, int userId, string prompt)
        {
            var eventToCreatePlan = await _eventRepository.GetEventWithDetailsAsync(eventId);
            if (eventToCreatePlan == null)
                throw new KeyNotFoundException("Event not found");
            
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            Console.WriteLine($"Generating plan for event: {eventToCreatePlan.Title} with prompt: {prompt}");

            var planText = await _planGenerator.GeneratePlanAsync(eventToCreatePlan, prompt);
            LLMGeneratedPlan generatedPlan = new()
            {
                EventId = eventId,
                PlanText = planText,
                CreatedAt = DateTime.UtcNow,
                Event = eventToCreatePlan,
                Generator = user
            };

            eventToCreatePlan.GeneratedPlans.Add(generatedPlan);

            return await _eventRepository.UpdateAsync(eventToCreatePlan);
        }

    }
}