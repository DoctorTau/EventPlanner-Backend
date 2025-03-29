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
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITelegramUserAccessor _telegramUserAccessor;


        public UserController(IUserService userService, ITelegramUserAccessor telegramUserAccessor)
        {
            _userService = userService;
            _telegramUserAccessor = telegramUserAccessor;
        }

        [HttpPost("auth")]
        public async Task<IActionResult> AuthenticateAsync()
        {
            try
            {
                var user = await _userService.GetUserByTelegramIdAsync(_telegramUserAccessor.User.Id);
                return Ok("User authenticated");
            }
            catch (KeyNotFoundException)
            {
                var userCreateDto = new UserDto
                {
                    TelegramId = _telegramUserAccessor.User.Id,
                    Username = _telegramUserAccessor.User.Username ?? string.Empty,
                    FirstName = _telegramUserAccessor.User.FirstName,
                    LastName = _telegramUserAccessor.User.LastName ?? string.Empty,
                };
                var user = await _userService.CreateUserAsync(userCreateDto);
                return Ok("Users created");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("availability")]
        public async Task<IActionResult> GetAvailabilityAsync()
        {
            try
            {
                var user = await _userService.GetUserByTelegramIdAsync(_telegramUserAccessor.User.Id);
                var availabilities = await _userService.GetUserAvailabilitiesAsync(user.Id);
                List<UserAvailabilityDto> availabilitiesDto =
                    availabilities.Select(ua => new UserAvailabilityDto
                    {
                        AvailableDate = ua.AvailableDate,
                        StartTime = ua.StartTime,
                        EndTime = ua.EndTime
                    }).ToList();
                return Ok(availabilitiesDto);
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

        [HttpPost("availability")]
        public async Task<IActionResult> AddAvailabilityAsync([FromBody] List<DateTime> availabilityDates)
        {
            try
            {
                var user = await _userService.GetUserByTelegramIdAsync(_telegramUserAccessor.User.Id);
                foreach (var date in availabilityDates)
                {
                    Console.WriteLine(date);
                    var userAvailabilityDto = new UserAvailabilityDto
                    {
                        AvailableDate = date,
                        StartTime = new TimeSpan(0, 0, 0),
                        EndTime = new TimeSpan(23, 59, 59)
                    };
                    await _userService.AddUserAvailabilityAsync(user.Id, userAvailabilityDto);
                }
                return Ok("Availability added");
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

        [HttpDelete("availability")]
        public async Task<IActionResult> DeleteAvailabilityAsync(List<DateTime> dateTime)
        {
            try
            {
                var user = await _userService.GetUserByTelegramIdAsync(_telegramUserAccessor.User.Id);
                foreach (var date in dateTime)
                {
                    await _userService.DeleteUserAvailabilityAsync(user.Id, date);
                }
                return Ok("Availability deleted");
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