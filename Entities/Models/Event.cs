using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventPlanner.Entities.Models
{
    public class Event
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CreatorId { get; set; }

        [Required]
        public long TelegramChatId { get; set; }

        [Required, MaxLength(255)]
        public required string Title { get; set; }

        public required string Description { get; set; } = string.Empty;

        public DateTime? EventDate { get; set; }

        public string? Location { get; set; } = null;

        public int? TimeVotingId { get; set; }
        public int? PlaceVotingId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public virtual required User Creator { get; set; }

        [ForeignKey("TimeVotingId")]
        public virtual Voting? TimeVoting { get; set; }
        [ForeignKey("PlaceVotingId")]
        public virtual Voting? PlaceVoting { get; set; }

        public virtual required ICollection<Participant> Participants { get; set; }
        public virtual required ICollection<TaskItem> Tasks { get; set; }
        public virtual required ICollection<EventDocument> Files { get; set; }
        public virtual required ICollection<Vote> Votes { get; set; }
        public virtual required ICollection<LLMGeneratedPlan> GeneratedPlans { get; set; }
    }
}
