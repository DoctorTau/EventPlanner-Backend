using EventPlanner.Business;
using EventPlanner.Entities.Models;
using EventPlanner.Entities.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TgMiniAppAuth;
using TgMiniAppAuth.AuthContext;

namespace EventPlanner_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly IUserService _userService;
        private readonly ITelegramUserAccessor _telegramUserAccessor;

        public EventController(IEventService eventService, ITelegramUserAccessor telegramUserAccessor, IUserService userService)
        {
            _eventService = eventService;
            _telegramUserAccessor = telegramUserAccessor;
            _userService = userService;
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
                        TelegramId = user.TelegramId,
                        Username = user.Username,
                        FirstName = user.FirstName,
                        LastName = user.LastName
                    });
                }

                EventWithParticipantsDto eventWithParticipantsDto = new EventWithParticipantsDto
                {
                    Id = @event.Id,
                    Title = @event.Title,
                    TelegramChatId = @event.TelegramChatId,
                    EventDate = @event.EventDate,
                    Location = @event.Location,
                    Description = @event.Description,
                    Participants = participantsDto
                };
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
                List<EventResponseDto> eventsDto = events.Select(e => new EventResponseDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    TelegramChatId = e.TelegramChatId,
                    EventDate = e.EventDate,
                    Location = e.Location,
                    Description = e.Description
                }).ToList();
                return Ok(eventsDto);
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
    }
}
