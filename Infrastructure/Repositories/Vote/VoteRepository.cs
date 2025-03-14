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

        public async Task<IEnumerable<Vote>> GetVotesByVotingAsync(int votingId)
        {
            return await _context.Votes.Where(v => v.VotingId == votingId).ToListAsync();
        }

        public async Task<Vote> GetUserVoteAsync(int votingId, int userId)
        {
            var vote = await _context.Votes.FirstOrDefaultAsync(v => v.VotingId == votingId && v.UserId == userId);

            if (vote == null)
                throw new KeyNotFoundException($"Vote with voting id {votingId} and user id {userId} not found");

            return vote;
        }

        public async Task<string> GetMostPopularVoteOptionAsync(int votingId)
        {
            var vote = await _context.Votes
                .Where(v => v.VotingId == votingId)
                .GroupBy(v => v.VoteOption)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefaultAsync();

            if (vote == null || vote == string.Empty)
                throw new KeyNotFoundException($"Vote with event id {votingId} not found");

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