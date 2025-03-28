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
        private readonly string _voteCreationUrl;

        public PollService(
            IVoteRepository voteRepository,
            IPollRepository votingRepository,
            IEventsRepository eventRepository,
            IUserRepository userRepository,
            IUserAvailabilityRepository userAvailabilityRepository,
            IConfiguration configuration,
            IParticipantRepository participantRepository)
        {
            _voteRepository = voteRepository;
            _pollRepository = votingRepository;
            _eventRepository = eventRepository;
            _userRepository = userRepository;
            _userAvailabilityRepository = userAvailabilityRepository;
            _voteCreationUrl = configuration["TelegramBotUrl"] ?? throw new ArgumentNullException("VoteCreationUrl configuration is missing");
            _participantRepository = participantRepository;

            _voteCreationUrl += "/start-vote";
        }

        public async Task<Poll> CreatePollAsync(PollCreateDto pollCreateDto)
        {
            Poll poll = new()
            {
                EventId = pollCreateDto.EventId,
                CreatedAt = DateTime.UtcNow,
                Options = pollCreateDto.Options,
                Event = await _eventRepository.GetByIdAsync(pollCreateDto.EventId),
                Votes = []
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


            return await SavePollToDbAsync(poll, @event);
        }

        private async Task<List<string>> GetOptionsFromDates(int eventId)
        {
            List<Participant> participants = (List<Participant>)await _participantRepository.GetParticipantsByEventIdAsync(eventId);
            Dictionary<int, List<UserAvailability>> usersAvailabilities = [];
            foreach (var participant in participants)
            {
                User user = await _userRepository.GetByIdAsync(participant.UserId);
                List<UserAvailability> userAvailability = await _userAvailabilityRepository.GetByUserIdAsync(user.Id);
                usersAvailabilities.Add(user.Id, userAvailability);
            }

            int totalParticipants = usersAvailabilities.Count;

            // Group availability by date
            var dateGroups = usersAvailabilities
                .SelectMany(kv => kv.Value.Select(ua => new { kv.Key, ua.AvailableDate }))
                .GroupBy(x => x.AvailableDate)
                .Select(g => new { Date = g.Key, UserCount = g.Select(x => x.Key).Distinct().Count() })
                .OrderByDescending(g => g.UserCount)
                .ThenBy(g => g.Date)
                .ToList();

            // Get dates with all participants
            var commonDates = dateGroups.Where(dg => dg.UserCount == totalParticipants).ToList();

            // If more than 7 dates with all participants, pick first 7
            if (commonDates.Count >= 7)
                return commonDates.Take(7).Select(d => d.Date.ToString("yyyy MM dd")).ToList();


            // If dates less than 2, reduce threshold to 80%
            if (commonDates.Count < 2)
            {
                int threshold80 = (int)Math.Ceiling(totalParticipants * 0.8);
                commonDates = dateGroups.Where(dg => dg.UserCount >= threshold80).ToList();

                // If still fewer than 3, reduce threshold to 50%
                if (commonDates.Count < 2)
                {
                    int threshold50 = (int)Math.Ceiling(totalParticipants * 0.5);
                    commonDates = dateGroups.Where(dg => dg.UserCount >= threshold50).ToList();
                }
            }

            // Take closest 7 dates or fewer
            return commonDates.Take(7).Select(d => d.Date.ToString("yyyy MM dd")).ToList();
        }

        private async Task<Poll> SavePollToDbAsync(Poll poll, Event @event)
        {
            Poll createdPoll = await _pollRepository.CreateAsync(poll);
            BotPollCreateDto botPollCreateDto = new()
            {
                votingId = createdPoll.Id,
                options = poll.Options,
                chatId = @event.TelegramChatId
            };

            try
            {
                await SendVoteCreationRequestAsync(botPollCreateDto);
            }
            catch (HttpRequestException)
            {
                await _pollRepository.DeleteAsync(createdPoll);
                throw;
            }

            return createdPoll;
        }

        private async Task SendVoteCreationRequestAsync(BotPollCreateDto botPollCreateDto)
        {
            using var httpClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, _voteCreationUrl);
            var content = new StringContent(JsonSerializer.Serialize(botPollCreateDto), Encoding.UTF8, "application/json");
            request.Content = content;
            var response = await httpClient.SendAsync(request);
            Console.WriteLine(content);
            response.EnsureSuccessStatusCode();
        }

        public async Task<Vote> CreateVoteAsync(VoteCreateDto voteCreateDto)
        {
            Vote vote = new()
            {
                PollId = voteCreateDto.VotingId,
                UserId = voteCreateDto.UserId,
                Type = voteCreateDto.Type,
                CreatedAt = DateTime.UtcNow,
                VoteOption = voteCreateDto.VoteOption,
                Poll = await _pollRepository.GetByIdAsync(voteCreateDto.VotingId),
                User = await _userRepository.GetByIdAsync(voteCreateDto.UserId)
            };

            await _voteRepository.CreateAsync(vote);
            return vote;
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

        public async Task<Poll> GetPollByEventIdAsync(int eventId)
        {
            var poll = await _pollRepository.GetPollByEventIdAsync(eventId);

            return poll;
        }

        public async Task<List<Vote>> GetVotesAsync(int eventId)
        {
            return await _pollRepository.GetVotesAsync(eventId);
        }

    }

    internal class BotPollCreateDto
    {
        public int votingId { get; set; }
        public required List<string> options { get; set; }
        public long chatId { get; set; }
    }
}