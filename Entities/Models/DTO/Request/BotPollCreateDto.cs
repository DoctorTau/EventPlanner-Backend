namespace EventPlanner.Entities.Models.Dto
{
    public class BotPollCreateDto
    {
        public int votingId { get; set; }
        public required List<string> options { get; set; }
        public long chatId { get; set; }
    }
}