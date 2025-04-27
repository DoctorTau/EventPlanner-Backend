using System.ComponentModel.DataAnnotations;
using System.Reflection;
using EventPlanner.Business;
using EventPlanner.Entities.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TgMiniAppAuth;
using TgMiniAppAuth.AuthContext;

namespace EventPlanner.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = TgMiniAppAuthConstants.AuthenticationScheme)]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly IUserService _userService;
        private readonly IEventService _eventsService;
        private readonly ITelegramUserAccessor _telegramUserAccessor;

        public TaskController(ITaskService taskService, ITelegramUserAccessor telegramUserAccessor, IUserService userService, IEventService eventsService)
        {
            _taskService = taskService;
            _telegramUserAccessor = telegramUserAccessor;
            _userService = userService;
            _eventsService = eventsService;
        }

        /// <summary>
        /// Creates a new task based on the provided task creation data.
        /// </summary>
        /// <param name="taskCreateDto">The data transfer object containing the details of the task to be created.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the created task if successful, 
        /// or a <see cref="BadRequestObjectResult"/> with an error message if the operation fails.
        /// </returns>
        [HttpPost("create")]
        public async Task<IActionResult> CreateTaskAsync([FromBody] TaskCreateDto taskCreateDto)
        {
            try
            {
                await UserIsParticipantAsync(taskCreateDto.EventId, _telegramUserAccessor.User.Id);

                var task = await _taskService.CreateTaskAsync(taskCreateDto);
                return Ok(task);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Event with id {taskCreateDto.EventId} not found");
            }
            catch (InvalidOperationException)
            {
                return BadRequest($"User with id {_telegramUserAccessor.User.Id} is not a participant of the event with id {taskCreateDto.EventId}");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{taskId}")]
        public async Task<IActionResult> GetTaskByIdAsync(int taskId)
        {
            try
            {
                var task = await _taskService.GetTaskByIdAsync(taskId);
                await UserIsParticipantAsync(task.EventId, _telegramUserAccessor.User.Id);
                return Ok(task);
            }
            catch (InvalidOperationException)
            {
                return BadRequest($"User with id {_telegramUserAccessor.User.Id} is not a participant of the event with id {taskId}");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{taskId}")]
        public async Task<IActionResult> DeleteTaskAsync(int taskId)
        {
            try
            {
                var task = await _taskService.GetTaskByIdAsync(taskId);
                await UserIsParticipantAsync(task.EventId, _telegramUserAccessor.User.Id);
                await _taskService.DeleteTaskAsync(taskId);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Task with id {taskId} not found");
            }
            catch (InvalidOperationException)
            {
                return BadRequest($"User with id {_telegramUserAccessor.User.Id} is not a participant of the event with id {taskId}");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetTasksByUserIdAsync()
        {
            try
            {
                var user = await _userService.GetUserByTelegramIdAsync(_telegramUserAccessor.User.Id);
                var tasks = await _taskService.GetTasksByUserIdAsync(user.Id);
                return Ok(tasks);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"User with id {_telegramUserAccessor.User.Id} not found");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("event/{eventId}")]
        public async Task<IActionResult> GetTasksByEventIdAsync(int eventId)
        {
            try
            {
                await UserIsParticipantAsync(eventId, _telegramUserAccessor.User.Id);
                var tasks = await _taskService.GetTasksByEventIdAsync(eventId);
                List<TaskResponseDto> taskDtos = tasks.Select(t => new TaskResponseDto(t)).ToList();
                return Ok(taskDtos);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{taskId}")]
        public async Task<IActionResult> UpdateTaskAsync(int taskId, [FromBody] TaskUpdateDto taskUpdateDto)
        {
            try
            {
                var task = await _taskService.GetTaskByIdAsync(taskId);
                await UserIsParticipantAsync(task.EventId, _telegramUserAccessor.User.Id);

                task = await _taskService.UpdateTaskAsync(taskId, taskUpdateDto);
                return Ok(new TaskResponseDto(task));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("statuses")]
        public ActionResult<List<object>> GetTaskStatusesAsync()
        {
            try
            {
                var taskStatuses = Enum.GetValues(typeof(Entities.Models.TaskItemStatus))
                      .Cast<Entities.Models.TaskItemStatus>()
                      .Select(e => new
                      {
                          value = e.ToString(),
                          displayName = e.GetType()
                                         .GetMember(e.ToString())
                                         .First()
                                         .GetCustomAttribute<DisplayAttribute>()?.Name ?? e.ToString()
                      })
                      .ToList<object>();
                return Ok(taskStatuses);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        private async Task UserIsParticipantAsync(int eventId, long userTelegramId)
        {
            var user = await _userService.GetUserByTelegramIdAsync(userTelegramId);
            if (user == null)
                throw new KeyNotFoundException($"User with telegram id {userTelegramId} not found");

            var userId = user.Id;
            var eventItem = await _eventsService.GetEventWithParticipantsAsync(eventId);
            if (eventItem == null)
                throw new KeyNotFoundException($"Event with id {eventId} not found");
            if (!eventItem.Participants.Any(p => p.UserId == userId))
                throw new InvalidOperationException($"User with id {userId} is not a participant of the event with id {eventId}");
        }

    }
}