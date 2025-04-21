using EventPlanner.Entities.Models;

namespace EventPlanner.Business
{
    public interface IPlanGenerator
    {
        Task<string> GeneratePlanAsync(Event eventToAddPlan, string prompt);
        Task<string> ModifyPlanAsync(Event eventToModifyPlan, string planToModify, string prompt);
    }
}