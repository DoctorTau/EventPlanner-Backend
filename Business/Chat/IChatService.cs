using EventPlanner.Entities.Models.Dto;

namespace EventPlanner.Business
{
    public interface IChatService
    {
        Task SendSummaryMessageAsync(int eventId);
        Task CreatePollAsync(BotPollCreateDto pollCreateDto);
    }
}