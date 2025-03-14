using EventPlanner.Business;
using Microsoft.AspNetCore.Mvc;

namespace EventPlanner_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("auth")]
        public IActionResult Authenticate()
        {
            // Authentication logic will be implemented here
            return Ok();
        }
    }
}