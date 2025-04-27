
namespace EventPlanner.Entities.Models.Dto
{
    public class TaskCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public int EventId { get; set; }
    }

    public class TaskUpdateDto
    {
        public string? Title { get; set; }
        public int? AssignedTo { get; set; }
        public TaskItemStatus? Status { get; set; }
    }
}