using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventPlanner.Entities.Models
{
    public class Vote
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EventId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public VoteType Type { get; set; } = VoteType.Date;

        [Required, MaxLength(255)]
        public required string VoteOption { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("EventId")]
        public virtual required Event Event { get; set; }

        [ForeignKey("UserId")]
        public virtual required User User { get; set; }
    }

    public enum VoteType
    {
        Date,
        Location
    }
}
