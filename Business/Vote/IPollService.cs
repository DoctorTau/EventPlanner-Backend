using EventPlanner.Entities.Models;
using EventPlanner.Entities.Models.Dto;

namespace EventPlanner.Business
{
    public interface IPollService
    {
        Task<Poll> CreatePollAsync(PollCreateDto pollCreateDto);
        Task<Poll> CreateDatePollAsync(int eventId);
        Task<Vote> CreateVoteAsync(VoteCreateDto voteCreateDto);

        Task<Poll> GetPollByEventIdAsync(int eventId);
        Task<List<Vote>> GetVotesAsync(int eventId);

        Task<string> GetMostVotedOptionAsync(int voteId);
    }
}