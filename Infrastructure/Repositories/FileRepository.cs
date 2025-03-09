using EventPlanner.Data;
using EventPlanner.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace EventPlanner.Repository
{
    public class FileRepository : IRepository<EventDocument>
    {
        private readonly IAppDbContext _context;

        public FileRepository(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<EventDocument> CreateAsync(EventDocument entity)
        {
            var result = await _context.EventDocuments.AddAsync(entity);
            await _context.SaveChangesAsync();
            return result.Entity;
        }

        public async Task DeleteAsync(EventDocument entity)
        {
            _context.EventDocuments.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<List<EventDocument>> GetAllAsync()
        {
            return await _context.EventDocuments.ToListAsync();
        }

        public async Task<EventDocument> GetByIdAsync(int id)
        {
            var entity = await _context.EventDocuments.FindAsync(id);

            if (entity == null)
                throw new KeyNotFoundException($"EventDocument with id {id} not found");

            return entity;
        }

        public async Task<EventDocument> UpdateAsync(EventDocument entity)
        {
            var result = _context.EventDocuments.Update(entity);
            await _context.SaveChangesAsync();
            return result.Entity;
        }
    }

}