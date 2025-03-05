using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventPlanner.Entities.Models
{
    public class UserAvailability
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        public DateTime AvailableDate { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        [ForeignKey("UserId")]
        public virtual required User User { get; set; }
    }
}
