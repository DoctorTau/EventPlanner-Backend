using EventPlanner.Business;
using EventPlanner.Entities.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace EventPlanner.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PollController : ControllerBase
    {
        private readonly IPollService _pollService;

        public PollController(IPollService pollService)
        {
            _pollService = pollService;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePoll([FromBody] PollCreateDto pollCreateDto)
        {
            try
            {
                var poll = await _pollService.CreatePollAsync(pollCreateDto);
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

        [HttpPost("{eventId}/start-date-poll")]
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

        [HttpGet("{eventId}")]
        public async Task<IActionResult> GetPoll(int eventId)
        {
            var poll = await _pollService.GetPollByEventIdAsync(eventId);
            return Ok(poll);
        }

        [HttpGet("{id}/votes")]
        public async Task<IActionResult> GetVotes(int id)
        {
            var votes = await _pollService.GetVotesAsync(id);
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
    }
}