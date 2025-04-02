using EventPlanner.Business;
using EventPlanner.Entities.Models;
using EventPlanner.Entities.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TgMiniAppAuth;
using TgMiniAppAuth.AuthContext;

namespace EventPlanner.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PollController : ControllerBase
    {
        private readonly IPollService _pollService;
        private readonly ITelegramUserAccessor _telegramUserAccessor;

        public PollController(IPollService pollService, ITelegramUserAccessor telegramUserAccessor)
        {
            _pollService = pollService;
            _telegramUserAccessor = telegramUserAccessor;
        }

        [HttpGet("{eventId}/get-location-poll")]
        [Authorize(AuthenticationSchemes = TgMiniAppAuthConstants.AuthenticationScheme)]
        public async Task<ActionResult<PollCreateDto>> GetLocationPoll(int eventId)
        {
            try
            {
                var poll = await _pollService.GetLocationPollAsync(eventId);
                return Ok(MapPollToDto(poll));
            }
            catch (KeyNotFoundException)
            {
                var poll = await _pollService.CreateLocationPollAsync(eventId);
                return Ok(MapPollToDto(poll));
            }
            catch (HttpRequestException)
            {
                return StatusCode(500, "Failed to create poll in Telegram bot");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{eventId}/add-location")]
        [Authorize(AuthenticationSchemes = TgMiniAppAuthConstants.AuthenticationScheme)]
        public async Task<IActionResult> AddLocation(int eventId, string location)
        {
            if (string.IsNullOrWhiteSpace(location))
            {
                return BadRequest("Location cannot be null or empty");
            }

            try
            {
                var poll = await _pollService.GetLocationPollAsync(eventId);
                if (poll == null)
                {
                    return NotFound("Poll not found");
                }
                var updatedPoll = await _pollService.AddOptionAsync(poll.Id, location);
                return Ok(MapPollToDto(updatedPoll));
            }
            catch (HttpRequestException)
            {
                return StatusCode(500, "Failed to create poll in Telegram bot");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("{eventId}/start-date-poll")]
        [Authorize(AuthenticationSchemes = TgMiniAppAuthConstants.AuthenticationScheme)]
        public async Task<IActionResult> StartDatePoll(int eventId)
        {
            try
            {
                var poll = await _pollService.CreateDatePollAsync(eventId);
                return Ok(poll);
            }
            catch (HttpRequestException)
            {
                return StatusCode(500, "Failed to create poll in Telegram bot");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpGet("{pollId}/votes")]
        public async Task<IActionResult> GetVotes(int pollId)
        {
            var votes = await _pollService.GetVotesAsync(pollId);
            return Ok(votes);
        }

        [HttpPost("vote")]
        public async Task<IActionResult> Vote([FromBody] VoteCreateDto voteCreateDto)
        {
            try
            {
                var vote = await _pollService.CreateVoteAsync(voteCreateDto);
                return Ok(vote);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        private PollCreateDto MapPollToDto(Poll poll)
        {
            return new PollCreateDto
            {
                EventId = poll.EventId,
                Options = poll.Options
            };
        }
    }
}