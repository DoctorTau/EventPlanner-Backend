using EventPlanner.Business;
using EventPlanner.Entities.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TgMiniAppAuth;
using TgMiniAppAuth.AuthContext;

namespace EventPlanner_Backend.Controllers
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
                // Log that user has logged in
                return Ok("User authenticated");
            }
            catch (KeyNotFoundException)
            {
                var userCreateDto = new UserCreateDto
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
    }
}