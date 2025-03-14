using EventPlanner.Data;
using EventPlanner.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace EventPlanner.Repository
{
    public class VotingsRepository : IVotingRepository
    {
        private readonly IAppDbContext _context;

        public VotingsRepository(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<Voting> CreateAsync(Voting entity)
        {
            var result = await _context.Votings.AddAsync(entity);
            await _context.SaveChangesAsync();
            return result.Entity;
        }

        public async Task DeleteAsync(Voting entity)
        {
            _context.Votings.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Voting>> GetAllAsync()
        {
            return await _context.Votings.ToListAsync();
        }

        public async Task<Voting> GetByIdAsync(int id)
        {
            var voting = await _context.Votings.FindAsync(id);

            if (voting == null)
                throw new KeyNotFoundException($"Voting with id {id} not found");

            return voting;
        }

        public async Task<List<Vote>> GetVotesAsync(int eventId)
        {
            return await _context.Votes.Where(v => v.VotingId == eventId).ToListAsync();
        }

        public async Task<Voting> UpdateAsync(Voting entity)
        {
            var result = _context.Votings.Update(entity);
            await _context.SaveChangesAsync();
            return result.Entity;
        }
    }
}