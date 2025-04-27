using System.Security.Cryptography.X509Certificates;
using EventPlanner.Entities.Models;
using EventPlanner.Entities.Models.Dto;
using EventPlanner.Repository;

namespace EventPlanner.Business
{
    public class TaskService : ITaskService
    {
        private readonly IRepository<TaskItem> _taskRepository;
        private readonly IEventsRepository _eventsRepository;
        private readonly IUserRepository _userRepository;

        public TaskService(IRepository<TaskItem> taskRepository, IEventsRepository eventsRepository, IUserRepository userRepository)
        {
            _taskRepository = taskRepository;
            _eventsRepository = eventsRepository;
            _userRepository = userRepository;
        }

        public async Task<TaskItem> CreateTaskAsync(TaskCreateDto taskCreateDto)
        {
            var taskItem = new TaskItem
            {
                Title = taskCreateDto.Title,
                EventId = taskCreateDto.EventId,
                Event = await _eventsRepository.GetByIdAsync(taskCreateDto.EventId),
                Assignee = null
            };

            return await _taskRepository.CreateAsync(taskItem);
        }

        public async Task DeleteTaskAsync(int taskId)
        {
            var taskItem = await _taskRepository.GetByIdAsync(taskId);
            if (taskItem == null)
                throw new KeyNotFoundException($"Task with id {taskId} not found");

            await _taskRepository.DeleteAsync(taskItem);
        }

        public async Task<List<TaskItem>> GetTasksByUserIdAsync(int userId)
        {
            var user = await _userRepository.GetUserWithDetailsAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"User with id {userId} not found");

            return user.AssignedTasks.ToList();
        }

        public async Task<TaskItem> GetTaskByIdAsync(int taskId)
        {
            var taskItem = await _taskRepository.GetByIdAsync(taskId);
            if (taskItem == null)
                throw new KeyNotFoundException($"Task with id {taskId} not found");

            return taskItem;
        }

        public async Task<List<TaskItem>> GetTasksByEventIdAsync(int eventId)
        {
            var eventItem = await _eventsRepository.GetEventWithDetailsAsync(eventId);
            if (eventItem == null)
                throw new KeyNotFoundException($"Event with id {eventId} not found");

            var tasks = new List<TaskItem>();
            foreach (var task in eventItem.Tasks)
            {
                if (await _taskRepository.GetByIdAsync(task.Id) != null)
                {
                    tasks.Add(task);
                }
            }
            return tasks;
        }

        public async Task<TaskItem> UpdateTaskAsync(int taskId, TaskUpdateDto taskUpdateDto)
        {
            var taskItem = await _taskRepository.GetByIdAsync(taskId);
            if (taskItem == null)
                throw new KeyNotFoundException($"Task with id {taskId} not found");
            if (taskUpdateDto.Title != null && taskUpdateDto.Title.Length > 0)
                taskItem.Title = taskUpdateDto.Title;
            if (taskUpdateDto.Status != null)
                taskItem.Status = taskUpdateDto.Status.Value;
            if (taskUpdateDto.AssignedTo != null)
                taskItem.AssignedTo = taskUpdateDto.AssignedTo.Value;

            await CheckParticipation(taskItem);

            return await _taskRepository.UpdateAsync(taskItem);
        }

        private async Task CheckParticipation(TaskItem taskItem)
        {
            var eventItem = await _eventsRepository.GetEventWithDetailsAsync(taskItem.EventId);
            if (eventItem == null)
                throw new KeyNotFoundException($"Event with id {taskItem.EventId} not found");
            if (!eventItem.Participants.Any(p => p.UserId == taskItem.AssignedTo))
                throw new KeyNotFoundException($"User with id {taskItem.AssignedTo} is not a participant of the event");
        }
    }
}