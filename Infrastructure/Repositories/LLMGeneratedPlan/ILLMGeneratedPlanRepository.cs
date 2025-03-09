using EventPlanner.Entities.Models;

namespace EventPlanner.Repository
{
    public interface ILLMGeneratedPlanRepository : IRepository<LLMGeneratedPlan>
    {
        public Task<LLMGeneratedPlan> GetByEventIdAsync(int eventId);
    }
}