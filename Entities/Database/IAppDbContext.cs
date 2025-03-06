using Microsoft.EntityFrameworkCore;
using EventPlanner.Entities.Models;

namespace EventPlanner.Data
{
    public interface IAppDbContext
    {
        DbSet<User> Users { get; }
        DbSet<Event> Events { get; }
        DbSet<Participant> Participants { get; }
        DbSet<TaskItem> TaskItems { get; }
        DbSet<EventDocument> EventDocuments { get; }
        DbSet<Vote> Votes { get; }
        DbSet<LLMGeneratedPlan> LLMGeneratedPlans { get; }
        DbSet<UserAvailability> UserAvailabilities { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
