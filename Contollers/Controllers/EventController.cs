using EventPlanner.Business;
using EventPlanner.Entities.Models.Dto;
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
    }
}
