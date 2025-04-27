namespace EventPlanner.Entities.Models.Dto
{
    public class TaskResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int EventId { get; set; }
        public int AssignedTo { get; set; }
        public TaskItemStatus Status { get; set; }

        public TaskResponseDto(TaskItem task)
        {
            Id = task.Id;
            Title = task.Title;
            EventId = task.EventId;
            AssignedTo = task.Assignee != null ? task.Assignee.Id : 0;
            Status = task.Status;
        }
    }
}