using EventPlanner.Data;
using EventPlanner.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace EventPlanner.Repository
{
    public class ParticipantRepository : IParticipantRepository
    {
        private readonly IAppDbContext _dbContext;

        public ParticipantRepository(IAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Participant> CreateAsync(Participant entity)
        {
            var result = await _dbContext.Participants.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task DeleteAsync(Participant entity)
        {
            _dbContext.Participants.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Participant>> GetAllAsync()
        {
            return await _dbContext.Participants.ToListAsync();
        }

        public async Task<Participant> GetByIdAsync(int id)
        {
            var participant = await _dbContext.Participants.FindAsync(id);

            if (participant == null)
                throw new KeyNotFoundException($"Participant with id {id} not found");

            return participant;
        }

        public async Task<IEnumerable<Participant>> GetParticipantsByEventIdAsync(int eventId)
        {
            var result = _dbContext.Participants.Where(p => p.EventId == eventId);
            return await result.ToListAsync();
        }

        public async Task<Participant> UpdateAsync(Participant entity)
        {
            var result = _dbContext.Participants.Update(entity);
            await _dbContext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task UpdateParticipantStatusAsync(int eventId, int userId, ParticipantStatus status)
        {
            var participant = await _dbContext.Participants.FirstOrDefaultAsync(p => p.EventId == eventId && p.UserId == userId);

            if (participant == null)
                throw new KeyNotFoundException($"Participant with event id {eventId} and user id {userId} not found");

            participant.Status = status;
            await _dbContext.SaveChangesAsync();
        }
    }
}