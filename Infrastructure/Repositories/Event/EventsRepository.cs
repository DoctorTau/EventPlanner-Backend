using EventPlanner.Data;
using EventPlanner.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace EventPlanner.Repository
{
    public class EventRepository : IEventsRepository
    {
        private readonly IAppDbContext _dbContext;

        public EventRepository(IAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Event> GetByIdAsync(int id)
        {
            var @event = await _dbContext.Events.FindAsync(id);

            if (@event == null)
                throw new KeyNotFoundException($"Event with id {id} not found");

            return @event;
        }

        public Task<List<Event>> GetAllAsync()
        {
            return _dbContext.Events.ToListAsync();
        }

        public async Task<Event> CreateAsync(Event @event)
        {
            var eventWithChatId = await _dbContext.Events.FirstOrDefaultAsync(e => e.TelegramChatId == @event.TelegramChatId);
            if (eventWithChatId != null && eventWithChatId.Id != 0)
                return eventWithChatId;

            var result = await _dbContext.Events.AddAsync(@event);
            await _dbContext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<Event> UpdateAsync(Event @event)
        {
            var result = _dbContext.Events.Update(@event);
            await _dbContext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task DeleteAsync(Event @event)
        {
            _dbContext.Events.Remove(@event);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Event>> GetByCreatorAsync(int creatorId)
        {
            var result = _dbContext.Events.Where(e => e.CreatorId == creatorId);
            return await result.ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var result = _dbContext.Events.Where(e => e.EventDate >= startDate && e.EventDate <= endDate);
            return await result.ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetByLocationAsync(string location)
        {
            var result = _dbContext.Events.Where(e => e.Location == location);
            return await result.ToListAsync();
        }

        public async Task<Event> GetEventWithDetailsAsync(int eventId)
        {
            var @event = await _dbContext.Events
                .Include(e => e.Participants)
                .Include(e => e.Tasks)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (@event == null)
                throw new KeyNotFoundException($"Event with id {eventId} not found");

            return @event;
        }

        public async Task<Event> GetByTelegramChatIdAsync(long telegramChatId)
        {
            var @event = await _dbContext.Events.Where(e => e.TelegramChatId == telegramChatId).FirstOrDefaultAsync();
            if (@event == null || @event.Id == 0)
                throw new KeyNotFoundException($"Event with telegram chat id {telegramChatId} not found");

            return @event;
        }

        public async Task<List<Event>> GetAllUsersEventsAsync(int userId)
        {
            return await _dbContext.Events.Where(e => e.Participants.Any(p => p.UserId == userId)).ToListAsync();
        }
    }
}