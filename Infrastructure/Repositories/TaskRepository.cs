
using EventPlanner.Data;
using EventPlanner.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace EventPlanner.Repository
{
    public class TaskRepository : IRepository<TaskItem>
    {
        private readonly IAppDbContext _context;

        public TaskRepository(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<TaskItem> CreateAsync(TaskItem entity)
        {
            var result = await _context.TaskItems.AddAsync(entity);
            await _context.SaveChangesAsync();
            return result.Entity;
        }

        public async Task DeleteAsync(TaskItem entity)
        {
            _context.TaskItems.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<List<TaskItem>> GetAllAsync()
        {
            return await _context.TaskItems.ToListAsync();
        }

        public async Task<TaskItem> GetByIdAsync(int id)
        {
            var entity = await _context.TaskItems.FindAsync(id);

            if (entity == null)
                throw new KeyNotFoundException($"TaskItem with id {id} not found");

            return entity;
        }

        public async Task<TaskItem> UpdateAsync(TaskItem entity)
        {
            var result = _context.TaskItems.Update(entity);
            await _context.SaveChangesAsync();
            return result.Entity;
        }
    }
}