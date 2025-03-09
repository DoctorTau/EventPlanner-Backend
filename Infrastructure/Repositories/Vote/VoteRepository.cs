using System.ComponentModel;
using EventPlanner.Data;
using EventPlanner.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace EventPlanner.Repository
{
    public class VoteRepository : IVoteRepository
    {
        private readonly IAppDbContext _context;

        public VoteRepository(IAppDbContext context)
        {
            _context = context;
        }

        async Task<IEnumerable<Vote>> IVoteRepository.GetVotesByEventIdAsync(int eventId)
        {
            return await _context.Votes.Where(v => v.EventId == eventId).ToListAsync();
        }

        async Task<Vote> IVoteRepository.GetUserVoteAsync(int eventId, int userId)
        {
            var vote = await _context.Votes.FirstOrDefaultAsync(v => v.EventId == eventId && v.UserId == userId);

            if (vote == null)
                throw new KeyNotFoundException($"Vote with event id {eventId} and user id {userId} not found");

            return vote;
        }

        async Task<string> IVoteRepository.GetMostPopularVoteOptionAsync(int eventId)
        {
            var vote = await _context.Votes
                .Where(v => v.EventId == eventId)
                .GroupBy(v => v.VoteOption)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefaultAsync();

            if (vote == null || vote == string.Empty)
                throw new KeyNotFoundException($"Vote with event id {eventId} not found");

            return vote;
        }

        public async Task<Vote> GetByIdAsync(int id)
        {
            var vote = await _context.Votes.FindAsync(id);

            if (vote == null)
                throw new KeyNotFoundException($"Vote with id {id} not found");

            return vote;
        }

        public async Task<List<Vote>> GetAllAsync()
        {
            return await _context.Votes.ToListAsync();
        }

        public async Task<Vote> CreateAsync(Vote entity)
        {
            var result = await _context.Votes.AddAsync(entity);
            await _context.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<Vote> UpdateAsync(Vote entity)
        {
            var result = _context.Votes.Update(entity);
            await _context.SaveChangesAsync();
            return result.Entity;
        }

        public Task DeleteAsync(Vote entity)
        {
            _context.Votes.Remove(entity);
            return _context.SaveChangesAsync();
        }
    }
}