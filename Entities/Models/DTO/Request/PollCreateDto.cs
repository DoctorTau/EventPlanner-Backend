using System.ComponentModel.DataAnnotations;

namespace EventPlanner.Entities.Models.Dto
{
    public class PollCreateDto
    {
        [Required]
        public int PollId { get; set; }
        [Required]
        public int EventId { get; set; }
        [Required]
        public List<string> Options { get; set; }
        [Required]
        public PollStatus Status { get; set; }

        public PollCreateDto(Poll poll)
        {
            PollId = poll.Id;
            EventId = poll.EventId;
            Options = poll.Options;
            Status = poll.Status;
        }
    }
}