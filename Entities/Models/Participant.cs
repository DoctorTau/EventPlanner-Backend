using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventPlanner.Entities.Models
{
    public class Participant
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EventId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public ParticipantStatus Status { get; set; } = ParticipantStatus.Invited;

        [ForeignKey("EventId")]
        public virtual required Event Event { get; set; }

        [ForeignKey("UserId")]
        public virtual required User User { get; set; }
    }

    public enum ParticipantStatus
    {
        Invited,
        Confirmed,
        Declined,
        Maybe
    }
}
