using EventPlanner.Data;
using EventPlanner.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace EventPlanner.Repository
{
    public class PollRepository : IPollRepository
    {
        private readonly IAppDbContext _context;

        public PollRepository(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<Poll> CreateAsync(Poll entity)
        {
            var result = await _context.Polls.AddAsync(entity);
            await _context.SaveChangesAsync();
            return result.Entity;
        }

        public async Task DeleteAsync(Poll entity)
        {
            _context.Polls.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Poll>> GetAllAsync()
        {
            return await _context.Polls.ToListAsync();
        }

        public async Task<Poll> GetByIdAsync(int id)
        {
            var voting = await _context.Polls.FindAsync(id);

            if (voting == null)
                throw new KeyNotFoundException($"Voting with id {id} not found");

            return voting;
        }

        public async Task<Poll> GetPollByEventIdAsync(int eventId)
        {
            var poll = await _context.Polls.FirstOrDefaultAsync(p => p.EventId == eventId);
            if (poll == null || poll.EventId != eventId)
                throw new KeyNotFoundException($"Poll with event id {eventId} not found");
            return poll;
        }

        public async Task<List<Vote>> GetVotesAsync(int eventId)
        {
            return await _context.Votes.Where(v => v.PollId == eventId).ToListAsync();
        }

        public async Task<Poll> UpdateAsync(Poll entity)
        {
            var result = _context.Polls.Update(entity);
            await _context.SaveChangesAsync();
            return result.Entity;
        }
    }
}