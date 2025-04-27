namespace EventPlanner.Business
{
    public interface IChatService
    {
        Task SendSummaryMessageAsync(int eventId);
    }
}