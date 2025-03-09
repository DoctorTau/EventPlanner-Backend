using System.Numerics;
using EventPlanner.Data;
using EventPlanner.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace EventPlanner.Repository
{
    public class LLMGeneratedPlanRepository : ILLMGeneratedPlanRepository
    {
        private readonly IAppDbContext _context;

        public LLMGeneratedPlanRepository(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<LLMGeneratedPlan> CreateAsync(LLMGeneratedPlan entity)
        {
            await _context.LLMGeneratedPlans.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(LLMGeneratedPlan entity)
        {
            _context.LLMGeneratedPlans.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<List<LLMGeneratedPlan>> GetAllAsync()
        {
            return await _context.LLMGeneratedPlans.ToListAsync();
        }

        public async Task<LLMGeneratedPlan> GetByIdAsync(int id)
        {
            var entity = await _context.LLMGeneratedPlans.FindAsync(id);

            if (entity == null)
                throw new KeyNotFoundException($"LLMGeneratedPlan with id {id} not found");

            return entity;
        }

        public async Task<LLMGeneratedPlan> GetByEventIdAsync(int eventId)
        {
            var result = await _context.LLMGeneratedPlans.FirstOrDefaultAsync(e => e.EventId == eventId);

            if (result == null || result.EventId != eventId)
                throw new KeyNotFoundException($"LLMGeneratedPlan with event id {eventId} not found");

            return result;
        }

        public async Task<LLMGeneratedPlan> UpdateAsync(LLMGeneratedPlan entity)
        {
            var result = _context.LLMGeneratedPlans.Update(entity);
            await _context.SaveChangesAsync();
            return result.Entity;
        }
    }
}