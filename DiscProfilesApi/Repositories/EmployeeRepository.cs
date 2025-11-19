using DiscProfilesApi.Interfaces;
using DiscProfilesApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DiscProfilesApi.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly DiscProfilesContext _context;

        public EmployeeRepository(DiscProfilesContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<employee>> GetAllAsync() =>
            await _context.employees.AsNoTracking().ToListAsync();

        public async Task<employee?> GetByIdAsync(int id) =>
            await _context.employees.AsNoTracking().FirstOrDefaultAsync(e => e.id == id);

        public async Task<employee> AddAsync(employee entity)
        {
            _context.employees.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<employee?> UpdateAsync(employee entity)
        {
            var existing = await _context.employees.FindAsync(entity.id);
            if (existing == null) return null;

            existing.email = entity.email;
            existing.phone = entity.phone;
            existing.company_id = entity.company_id;
            existing.person_id = entity.person_id;
            existing.department_id = entity.department_id;
            existing.position_id = entity.position_id;
            existing.disc_profile_id = entity.disc_profile_id;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.employees.FindAsync(id);
            if (existing == null) return false;
            _context.employees.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
