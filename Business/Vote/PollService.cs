using EventPlanner.Entities.Models;
using EventPlanner.Entities.Models.Dto;
using EventPlanner.Repository;

namespace EventPlanner.Business
{
    public class PollService : IPollService
    {
        private readonly IVoteRepository _voteRepository;
        private readonly IPollRepository _pollRepository;
        private readonly IEventsRepository _eventRepository;
        private readonly IUserRepository _userRepository;

        public PollService(
            IVoteRepository voteRepository,
            IPollRepository votingRepository,
            IEventsRepository eventRepository,
            IUserRepository userRepository)
        {
            _voteRepository = voteRepository;
            _pollRepository = votingRepository;
            _eventRepository = eventRepository;
            _userRepository = userRepository;
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

            await _pollRepository.CreateAsync(poll);
            return poll;
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
}