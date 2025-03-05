using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventPlanner.Entities.Models
{
    public class Task
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EventId { get; set; }

        public int? AssignedTo { get; set; }

        [Required, MaxLength(255)]
        public required string Title { get; set; }

        [Required]
        public TaskStatus Status { get; set; } = TaskStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("EventId")]
        public virtual required Event Event { get; set; }

        [ForeignKey("AssignedTo")]
        public virtual required User Assignee { get; set; }
    }

    public enum TaskStatus
    {
        Pending,
        InProgress,
        Completed
    }
}
