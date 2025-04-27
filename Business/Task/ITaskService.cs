using EventPlanner.Entities.Models;
using EventPlanner.Entities.Models.Dto;

namespace EventPlanner.Business
{
    public interface ITaskService
    {
        Task<TaskItem> CreateTaskAsync(TaskCreateDto taskCreateDto);
        Task<TaskItem> GetTaskByIdAsync(int taskId);
        Task<List<TaskItem>> GetTasksByEventIdAsync(int eventId);
        Task<List<TaskItem>> GetTasksByUserIdAsync(int userId);
        Task<TaskItem> UpdateTaskAsync(int taskId, TaskUpdateDto taskUpdateDto);
        Task DeleteTaskAsync(int taskId);
    }
}
