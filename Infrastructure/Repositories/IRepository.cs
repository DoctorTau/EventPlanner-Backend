namespace EventPlanner.Repository
{
    public interface IRepository<T> where T : class
    {
        public Task<T> GetByIdAsync(int id);

        public Task<List<T>> GetAllAsync();

        public Task<T> CreateAsync(T entity);

        public Task<T> UpdateAsync(T entity);

        public Task DeleteAsync(T entity);
    }

}