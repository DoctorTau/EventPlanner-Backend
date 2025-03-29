using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventPlanner.Entities.Models
{
    public class Vote
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PollId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required, MaxLength(255)]
        public required string VoteOption { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("VoteId")]
        public virtual required Poll Poll { get; set; }

        [ForeignKey("UserId")]
        public virtual required User User { get; set; }
    }

}
