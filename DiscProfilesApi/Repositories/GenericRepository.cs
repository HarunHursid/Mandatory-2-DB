using DiscProfilesApi.Interfaces;
using DiscProfilesApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DiscProfilesApi.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly DiscProfilesContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(DiscProfilesContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync() =>
            await _dbSet.AsNoTracking().ToListAsync();

        public virtual async Task<T?> GetByIdAsync(int id) =>
            await _dbSet.FindAsync(id);

        public virtual async Task<T> AddAsync(T entity)
        {
            _dbSet.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<T?> UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<bool> DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null) return false;
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public virtual async Task<T?> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(e => 
                EF.Property<string>(e, "Email") == email);
        }
    }
}