using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventPlanner.Entities.Models
{
    public class TaskItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EventId { get; set; }

        public int? AssignedTo { get; set; }

        [Required, MaxLength(255)]
        public required string Title { get; set; }

        [Required]
        public TaskItemStatus Status { get; set; } = TaskItemStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("EventId")]
        public virtual required Event Event { get; set; }

        [ForeignKey("AssignedTo")]
        public virtual required User? Assignee { get; set; } = null;
    }

    public enum TaskItemStatus
    {
        [Display(Name = "Pending")]
        Pending,
        [Display(Name = "In Progress")]
        InProgress,
        [Display(Name = "Completed")]
        Completed
    }
}
