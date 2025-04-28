using System.Text;
using System.Text.Json;
using EventPlanner.Entities.Models;
using EventPlanner.Entities.Models.Dto;
using EventPlanner.Repository;
using Microsoft.Extensions.Configuration;
using System.Net.Http;

namespace EventPlanner.Business
{
    public class PollService : IPollService
    {
        private readonly IVoteRepository _voteRepository;
        private readonly IPollRepository _pollRepository;
        private readonly IEventsRepository _eventRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserAvailabilityRepository _userAvailabilityRepository;
        private readonly IParticipantRepository _participantRepository;
        private readonly IChatService _chatService;

        public PollService(
            IVoteRepository voteRepository,
            IPollRepository votingRepository,
            IEventsRepository eventRepository,
            IUserRepository userRepository,
            IUserAvailabilityRepository userAvailabilityRepository,
            IConfiguration configuration,
            IParticipantRepository participantRepository,
            IChatService chatService)
        {
            _voteRepository = voteRepository;
            _pollRepository = votingRepository;
            _eventRepository = eventRepository;
            _userRepository = userRepository;
            _userAvailabilityRepository = userAvailabilityRepository;
            _participantRepository = participantRepository;
            _chatService = chatService;
        }

        public async Task<Poll> CreatePollAsync(PollCreateDto pollCreateDto)
        {
            Poll poll = new()
            {
                EventId = pollCreateDto.EventId,
                CreatedAt = DateTime.UtcNow,
                Options = pollCreateDto.Options,
                Event = await _eventRepository.GetByIdAsync(pollCreateDto.EventId),
                Votes = new List<Vote>()
            };

            Event @event = await _eventRepository.GetByIdAsync(pollCreateDto.EventId);

            return await SavePollToDbAsync(poll, @event);
        }

        public async Task<Poll> CreateDatePollAsync(int eventId)
        {
            Event @event = await _eventRepository.GetByIdAsync(eventId);
            List<string> options = await GetOptionsFromDates(eventId);

            Poll poll = new()
            {
                EventId = eventId,
                CreatedAt = DateTime.UtcNow,
                Options = options,
                Event = await _eventRepository.GetByIdAsync(eventId),
                Votes = []
            };

            await SavePollToDbAsync(poll, @event);

            await StartInChatPollAsync(poll.Id);

            @event.TimePollId = poll.Id;
            await _eventRepository.UpdateAsync(@event);
            return poll;
        }

        private async Task<List<string>> GetOptionsFromDates(int eventId)
        {
            List<Participant> participants = (List<Participant>)await _participantRepository.GetParticipantsByEventIdAsync(eventId);
            Dictionary<int, List<UserAvailability>> usersAvailabilities = new Dictionary<int, List<UserAvailability>>();
            foreach (var participant in participants)
            {
                User user = await _userRepository.GetByIdAsync(participant.UserId);
                List<UserAvailability> userAvailability = await _userAvailabilityRepository.GetByUserIdAsync(user.Id);
                usersAvailabilities.Add(user.Id, userAvailability);
            }

            int totalParticipants = usersAvailabilities.Count;

            var dateGroups = usersAvailabilities
                .SelectMany(kv => kv.Value.Select(ua => new { kv.Key, ua.AvailableDate }))
                .GroupBy(x => x.AvailableDate)
                .Select(g => new { Date = g.Key, UserCount = g.Select(x => x.Key).Distinct().Count() })
                .OrderByDescending(g => g.UserCount)
                .ThenBy(g => g.Date)
                .ToList();

            var commonDates = dateGroups.Where(dg => dg.UserCount == totalParticipants).ToList();

            if (commonDates.Count >= 7)
                return commonDates.Take(7).Select(d => d.Date.ToString("yyyy MM dd")).ToList();


            if (commonDates.Count < 2)
            {
                int threshold80 = (int)Math.Ceiling(totalParticipants * 0.8);
                commonDates = dateGroups.Where(dg => dg.UserCount >= threshold80).ToList();

                if (commonDates.Count < 2)
                {
                    int threshold50 = (int)Math.Ceiling(totalParticipants * 0.5);
                    commonDates = dateGroups.Where(dg => dg.UserCount >= threshold50).ToList();
                }
            }

            return commonDates.Take(7).Select(d => d.Date.ToString("yyyy MM dd")).ToList();
        }

        private async Task<Poll> SavePollToDbAsync(Poll poll, Event @event)
        {
            Poll createdPoll = await _pollRepository.CreateAsync(poll);

            return createdPoll;
        }

        private async Task StartInChatPollAsync(int pollId)
        {
            Poll poll = await _pollRepository.GetByIdAsync(pollId)
                         ?? throw new Exception($"Poll with id {pollId} not found");
            if (poll.Status == PollStatus.Open)
                throw new Exception($"Poll with id {pollId} is already open");
            Event @event = await _eventRepository.GetByIdAsync(poll.EventId)
                         ?? throw new Exception($"Event with id {poll.EventId} not found");

            try
            {
                await _chatService.CreatePollAsync(new BotPollCreateDto
                {
                    votingId = poll.Id,
                    options = poll.Options,
                    chatId = @event.TelegramChatId
                });
            }
            catch (HttpRequestException)
            {
                throw;
            }

            poll.Status = PollStatus.Open;
            await _pollRepository.UpdateAsync(poll);
        }

        public async Task<Vote> CreateVoteAsync(VoteCreateDto voteCreateDto)
        {
            User user = await _userRepository.GetUserByTelegramIdAsync(voteCreateDto.UserId)
                         ?? throw new Exception($"User with tg id {voteCreateDto.UserId} not found");
            Poll poll = await _pollRepository.GetByIdAsync(voteCreateDto.PollId)
                         ?? throw new Exception($"Poll with id {voteCreateDto.PollId} not found");

            if (poll.Status != PollStatus.Open)
            {
                throw new Exception($"Poll with id {voteCreateDto.PollId} is not open");
            }

            Vote vote = new()
            {
                PollId = voteCreateDto.PollId,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                VoteOption = poll.Options[voteCreateDto.VoteIndex],
                Poll = poll,
                User = user
            };

            try
            {
                await _voteRepository.CreateAsync(vote);
                await CountVoteAsync(vote);
            }
            catch
            {
                await _voteRepository.DeleteAsync(vote);
                throw new Exception($"Failed to create vote for user with id {user.Id}");
            }
            return vote;
        }

        private async Task CountVoteAsync(Vote vote)
        {
            Poll poll = await _pollRepository.GetByIdAsync(vote.PollId)
                         ?? throw new Exception($"Poll with id {vote.PollId} not found");
            Event votedEvent = await _eventRepository.GetByIdAsync(poll.EventId)
                         ?? throw new Exception($"Event with id {poll.EventId} not found");

            if (votedEvent.TimePollId == poll.Id)
            {
                var mostVotedOption = await GetMostVotedOptionAsync(poll.Id);
                Console.WriteLine("Most voted option: " + mostVotedOption);

                if (!DateTime.TryParseExact(mostVotedOption, new[] { "yyyy MM dd", "yyyy-MM-dd", "yyyy/MM/dd" }, null, System.Globalization.DateTimeStyles.None, out var dateTime))
                {
                    throw new Exception($"Failed to parse the most voted option '{mostVotedOption}' as a date.");
                }

                dateTime = dateTime.ToUniversalTime();
                Console.WriteLine($"Parsed date: {dateTime}");
                votedEvent.EventDate = dateTime;
            }
            else if (votedEvent.LocationPollId == poll.Id)
            {
                var mostVotedOption = await GetMostVotedOptionAsync(poll.Id);
                votedEvent.Location = mostVotedOption;
            }
            else
            {
                throw new Exception($"Poll with id {poll.Id} is not related to the event with id {votedEvent.Id}");
            }

            await _eventRepository.UpdateAsync(votedEvent);
        }

        public async Task<Poll> CreateLocationPollAsync(int eventId)
        {
            Event @event = await _eventRepository.GetByIdAsync(eventId);
            if (@event.LocationPollId != null)
                throw new Exception($"Event with id {eventId} already has a location poll");

            Poll poll = new()
            {
                EventId = eventId,
                CreatedAt = DateTime.UtcNow,
                Options = new List<string>(),
                Event = @event,
                Votes = new List<Vote>()
            };

            await SavePollToDbAsync(poll, @event);
            @event.LocationPollId = poll.Id;
            await _eventRepository.UpdateAsync(@event);

            return poll;
        }

        public async Task<Poll> AddOptionAsync(int pollId, string option)
        {
            Poll poll = await _pollRepository.GetByIdAsync(pollId)
                         ?? throw new Exception($"Poll with id {pollId} not found");
            if (poll.Status == PollStatus.Open)
                throw new Exception($"Poll with id {pollId} is already open");

            // Add option if it doesn't exist
            if (!poll.Options.Contains(option))
            {
                poll.Options.Add(option);
                await _pollRepository.UpdateAsync(poll);
            }
            return poll;
        }

        public async Task<Poll> GetLocationPollAsync(int eventId)
        {
            Event @event = await _eventRepository.GetByIdAsync(eventId)
                         ?? throw new Exception($"Event with id {eventId} not found");
            if (@event.LocationPollId == null)
                throw new KeyNotFoundException($"Event with id {eventId} has no location poll");
            Poll poll = await _pollRepository.GetByIdAsync(@event.LocationPollId.Value)
                         ?? throw new KeyNotFoundException($"Poll with id {@event.LocationPollId} not found");
            return poll;
        }

        public async Task DeleteVoteAsync(int voteId)
        {
            var vote = await _voteRepository.GetByIdAsync(voteId)
                       ?? throw new Exception($"Vote with id {voteId} not found");
            await _voteRepository.DeleteAsync(vote);
        }

        public async Task<string> GetMostVotedOptionAsync(int voteId)
        {
            var votes = await _pollRepository.GetVotesAsync(voteId);
            var mostVotedOption = votes
                .GroupBy(v => v.VoteOption)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault();

            if (mostVotedOption == null)
                throw new Exception("No votes found");

            return mostVotedOption;
        }

        public async Task<List<Vote>> GetVotesAsync(int pollId)
        {
            var votes = await _voteRepository.GetVotesByPollAsync(pollId);
            return votes.ToList();
        }

        public async Task<Poll> StartPollAsync(int pollId)
        {
            await StartInChatPollAsync(pollId);
            return await _pollRepository.GetByIdAsync(pollId)
                   ?? throw new Exception($"Poll with id {pollId} not found");
        }
    }


}