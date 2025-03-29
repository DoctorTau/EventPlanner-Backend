using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventPlanner.Entities.Models
{
    public class Poll
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EventId { get; set; }

        [Required]
        public required List<string> Options { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("EventId")]
        public virtual required Event Event { get; set; }

        public virtual required ICollection<Vote> Votes { get; set; }
    }

}