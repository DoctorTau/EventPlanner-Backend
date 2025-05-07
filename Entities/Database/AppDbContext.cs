using Microsoft.EntityFrameworkCore;
using EventPlanner.Entities.Models;

namespace EventPlanner.Data
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<TaskItem> TaskItems { get; set; }
        public DbSet<EventDocument> EventDocuments { get; set; }
        public DbSet<Vote> Votes { get; set; }
        public DbSet<Poll> Polls { get; set; }
        public DbSet<LLMGeneratedPlan> LLMGeneratedPlans { get; set; }
        public DbSet<UserAvailability> UserAvailabilities { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("YourConnectionStringHere");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.TelegramId)
                .IsUnique();

            modelBuilder.Entity<Participant>()
                .HasOne(p => p.Event)
                .WithMany(e => e.Participants)
                .HasForeignKey(p => p.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Participant>()
                .HasOne(p => p.User)
                .WithMany(u => u.Participations)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.Event)
                .WithMany(e => e.Tasks)
                .HasForeignKey(t => t.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.Assignee)
                .WithMany(u => u.AssignedTasks)
                .HasForeignKey(t => t.AssignedTo)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<EventDocument>()
                .HasOne(f => f.Event)
                .WithMany(e => e.Files)
                .HasForeignKey(f => f.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EventDocument>()
                .HasOne(f => f.Uploader)
                .WithMany(u => u.UploadedFiles)
                .HasForeignKey(f => f.UploadedBy)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Vote>()
                .HasOne(v => v.User)
                .WithMany(u => u.Votes)
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LLMGeneratedPlan>()
                .HasOne(lp => lp.Event)
                .WithMany(e => e.GeneratedPlans)
                .HasForeignKey(lp => lp.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LLMGeneratedPlan>()
                .HasOne(lp => lp.Generator)
                .WithMany(u => u.GeneratedPlans)
                .HasForeignKey(lp => lp.GeneratedBy)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserAvailability>()
                .HasKey(ua => new { ua.UserId, ua.AvailableDate });

            modelBuilder.Entity<Poll>()
                .HasOne(p => p.Event)
                .WithMany()
                .HasForeignKey(p => p.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Event>()
                .HasOne(e => e.TimePoll)
                .WithOne()
                .HasForeignKey<Event>(e => e.TimePollId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Event>()
                .HasOne(e => e.LocationPoll)
                .WithOne()
                .HasForeignKey<Event>(e => e.LocationPollId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Vote>()
                .HasOne(v => v.Poll)
                .WithMany(voting => voting.Votes)
                .HasForeignKey(v => v.PollId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
