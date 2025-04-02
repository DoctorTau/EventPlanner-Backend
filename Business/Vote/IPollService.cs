using EventPlanner.Entities.Models;
using EventPlanner.Entities.Models.Dto;

namespace EventPlanner.Business
{
    public interface IPollService
    {
        Task<Poll> CreatePollAsync(PollCreateDto pollCreateDto);
        Task<Poll> CreateDatePollAsync(int eventId);
        Task<Poll> CreateLocationPollAsync(int eventId);
        Task<Vote> CreateVoteAsync(VoteCreateDto voteCreateDto);

        Task<Poll> GetLocationPollAsync(int eventId);
        Task<Poll> AddOptionAsync(int pollId, string option);

        Task<List<Vote>> GetVotesAsync(int pollId);

        Task<string> GetMostVotedOptionAsync(int voteId);
    }
}