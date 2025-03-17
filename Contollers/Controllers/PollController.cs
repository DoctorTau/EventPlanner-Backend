using EventPlanner.Business;
using EventPlanner.Entities.Models.Dto;
using EventPlanner.Repository;
using Microsoft.AspNetCore.Mvc;

namespace EventPlanner.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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

        [HttpPost("{id}/vote")]
        public async Task<IActionResult> Vote(int id, [FromBody] VoteCreateDto voteCreateDto)
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