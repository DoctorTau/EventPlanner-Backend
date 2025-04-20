using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

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

        public GroupEventType EventType { get; set; } = GroupEventType.None;

        public int? TimePollId { get; set; }
        public int? LocationPollId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public virtual required User Creator { get; set; }

        [ForeignKey("TimePollId")]
        public virtual Poll? TimePoll { get; set; }
        [ForeignKey("LocationPollId")]
        public virtual Poll? LocationPoll { get; set; }

        public virtual required ICollection<Participant> Participants { get; set; }
        public virtual required ICollection<TaskItem> Tasks { get; set; }
        public virtual required ICollection<EventDocument> Files { get; set; }
        public virtual required ICollection<Vote> Votes { get; set; }
        public virtual required ICollection<LLMGeneratedPlan> GeneratedPlans { get; set; }
    }

    public enum GroupEventType
    {
        [Display(Name = "None")]
        None,
        [Display(Name = "Party")]
        Party,
        [Display(Name = "Bar Outing")]
        BarOuting,
        [Display(Name = "Movie Night")]
        MovieNight,
        [Display(Name = "Board Game Night")]
        BoardGameNight,
        [Display(Name = "Trip")]
        Trip,
        [Display(Name = "Team Sports Match")]
        TeamSportsMatch,
        [Display(Name = "Online Hangout")]
        OnlineHangout,
        [Display(Name = "Workshop")]
        Workshop,
        [Display(Name = "Cooking Night")]
        CookingNight,
        [Display(Name = "Cultural Event")]
        CulturalEvent
    }
}
