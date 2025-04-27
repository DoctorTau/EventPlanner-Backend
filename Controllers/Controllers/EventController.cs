using System.ComponentModel.DataAnnotations;
using System.Reflection;
using EventPlanner.Business;
using EventPlanner.Entities.Models;
using EventPlanner.Entities.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TgMiniAppAuth;
using TgMiniAppAuth.AuthContext;

namespace EventPlanner.Controllers.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly IUserService _userService;
        private readonly IChatService _chatService;
        private readonly ITelegramUserAccessor _telegramUserAccessor;

        public EventController(IEventService eventService, ITelegramUserAccessor telegramUserAccessor, IUserService userService, IChatService chatService)
        {
            _eventService = eventService;
            _telegramUserAccessor = telegramUserAccessor;
            _userService = userService;
            _chatService = chatService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateEventAsync([FromBody] EventCreateDto newEvent)
        {
            try
            {
                var @event = await _eventService.CreateEventAsync(newEvent);
                return Ok(@event);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{eventId}")]
        public async Task<ActionResult<EventWithParticipantsDto>> GetEventByIdAsync(int eventId)
        {
            try
            {
                var @event = await _eventService.GetEventWithParticipantsAsync(eventId);
                if (@event == null)
                    return NotFound("Event not found");

                List<UserDto> participantsDto = new List<UserDto>();
                foreach (var p in @event.Participants)
                {
                    var user = await _userService.GetUserAsync(p.UserId);
                    participantsDto.Add(new UserDto
                    {
                        Id = user.Id,
                        TelegramId = user.TelegramId,
                        Username = user.Username,
                        FirstName = user.FirstName,
                        LastName = user.LastName
                    });
                }

                EventWithParticipantsDto eventWithParticipantsDto = new EventWithParticipantsDto(@event, participantsDto);
                return Ok(eventWithParticipantsDto);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("getByTelegramChatId/{telegramChatId}")]
        public async Task<IActionResult> GetEventByTelegramChatIdAsync(long telegramChatId)
        {
            try
            {
                var @event = await _eventService.GetEventByTelegramChatIdAsync(telegramChatId);
                return Ok(@event);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("getUsersEvents")]
        [Authorize(AuthenticationSchemes = TgMiniAppAuthConstants.AuthenticationScheme)]
        public async Task<ActionResult<List<EventResponseDto>>> GetAllUsersEventsAsync()
        {
            try
            {
                var user = await _userService.GetUserByTelegramIdAsync(_telegramUserAccessor.User.Id);
                var events = await _eventService.GetAllUsersEventsAsync(user.Id);
                List<EventResponseDto> eventsDto = events.Select(e => new EventResponseDto(e)
                ).ToList();
                return Ok(eventsDto);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{eventId}/getEventPlan")]
        [Authorize(AuthenticationSchemes = TgMiniAppAuthConstants.AuthenticationScheme)]
        public async Task<ActionResult<string>> GetEventPlanAsync(int eventId)
        {
            try
            {
                var user = await _userService.GetUserByTelegramIdAsync(_telegramUserAccessor.User.Id);
                var @event = await _eventService.GetEventWithAllDetailsAsync(eventId);
                if (@event == null)
                    return NotFound("Event not found");
                if (@event.Participants.All(p => p.UserId != user.Id))
                    return BadRequest("User is not a participant of this event");

                if (@event.GeneratedPlans.Count == 0)
                    return NotFound("No plans generated for this event");

                return Ok(@event.GeneratedPlans.Last().PlanText);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{eventId}/updateEventDetails")]
        [Authorize(AuthenticationSchemes = TgMiniAppAuthConstants.AuthenticationScheme)]
        public async Task<IActionResult> UpdateEventDetailsAsync(int eventId, [FromBody] EventUpdateDto eventUpdateDto)
        {
            try
            {
                var user = await _userService.GetUserByTelegramIdAsync(_telegramUserAccessor.User.Id);
                var @event = await _eventService.GetEventWithAllDetailsAsync(eventId);
                if (@event == null)
                    return NotFound("Event not found");
                if (@event.Participants.All(p => p.UserId != user.Id))
                    return BadRequest("User is not a participant of this event");

                return Ok(await _eventService.UpdateEventAsync(eventId, eventUpdateDto));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("getEventTypes")]
        public ActionResult<List<object>> GetEventTypes()
        {
            try
            {
                var eventTypes = Enum.GetValues(typeof(GroupEventType))
                    .Cast<GroupEventType>()
                    .Select(e => new
                    {
                        value = e.ToString(),
                        displayName = e.GetType()
                                       .GetMember(e.ToString())
                                       .First()
                                       .GetCustomAttribute<DisplayAttribute>()?.Name ?? e.ToString()
                    })
                    .ToList<object>();

                return Ok(eventTypes);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpPost("{eventId}/join")]
        [Authorize(AuthenticationSchemes = TgMiniAppAuthConstants.AuthenticationScheme)]
        public async Task<IActionResult> JoinEventAsync(int eventId)
        {
            try
            {
                var user = await _userService.GetUserByTelegramIdAsync(_telegramUserAccessor.User.Id);
                await _eventService.AddParticipantAsync(eventId, user.Id);
                return Ok("User joined event");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("{eventTgId}/join/{userTgId}")]
        public async Task<IActionResult> JoinEventAsync(long eventTgId, long userTgId)
        {
            try
            {
                var @event = await _eventService.GetEventByTelegramChatIdAsync(eventTgId);
                var user = await _userService.GetUserByTelegramIdAsync(userTgId);
                await _eventService.AddParticipantAsync(@event.Id, user.Id);
                return Ok("User joined event");
            }
            catch (KeyNotFoundException)
            {
                return NotFound("User not found");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("{eventId}/generatePlan")]
        [Authorize(AuthenticationSchemes = TgMiniAppAuthConstants.AuthenticationScheme)]
        public async Task<IActionResult> GeneratePlanAsync(int eventId, [FromBody] string prompt)
        {
            try
            {
                var user = await _userService.GetUserByTelegramIdAsync(_telegramUserAccessor.User.Id);
                var @event = await _eventService.GeneratePlanAsync(eventId, user.Id, prompt);
                return Ok(@event);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{eventId}/updatePlan")]
        [Authorize(AuthenticationSchemes = TgMiniAppAuthConstants.AuthenticationScheme)]
        public async Task<IActionResult> UpdatePlanAsync(int eventId, [FromBody] PlanUpdateDto planUpdateDto)
        {
            try
            {
                var user = await _userService.GetUserByTelegramIdAsync(_telegramUserAccessor.User.Id);
                var @event = await _eventService.ModifyPlanAsync(eventId, user.Id, planUpdateDto.original_plan, planUpdateDto.user_comment);
                return Ok(@event.GeneratedPlans.Last().PlanText);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("{eventId}/SendMessage")]
        [Authorize(AuthenticationSchemes = TgMiniAppAuthConstants.AuthenticationScheme)]
        public async Task<IActionResult> SendMessageAsync(int eventId)
        {
            try
            {
                var user = await _userService.GetUserByTelegramIdAsync(_telegramUserAccessor.User.Id);

                var @event = await _eventService.GetEventWithParticipantsAsync(eventId);
                if (user == null || @event == null)
                    return NotFound("Event not found");

                if (@event.Participants.All(p => p.UserId != user.Id))
                    return BadRequest("User is not a participant of this event");

                await _chatService.SendSummaryMessageAsync(eventId);
                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}