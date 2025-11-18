using DiscProfilesApi.Interfaces;
using DiscProfilesApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DiscProfilesApi.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly DiscProfilesContext _context;

        public EmployeeService(DiscProfilesContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<employee>> GetAllAsync()
        {
            var entities = await _context.employees.AsNoTracking().ToListAsync();
            return entities.Select(MapToDto);
        }

        public async Task<employee?> GetByIdAsync(int id)
        {
            var entity = await _context.employees.AsNoTracking().FirstOrDefaultAsync(e => e.id == id);
            return entity == null ? null : MapToDto(entity);
        }

        public async Task<employee> CreateAsync(employee employee)
        {
            var entity = MapToEntity(employee);
            _context.employees.Add(entity);
            await _context.SaveChangesAsync();
            return MapToDto(entity);
        }

        public async Task<employee> UpdateAsync(employee employee)
        {
            var entity = await _context.employees.FindAsync(employee.id);
            if (entity == null)
                throw new KeyNotFoundException("Employee not found");

            entity.email = employee.email;
            entity.phone = employee.phone;
            entity.company_id = employee.company_id;
            entity.person_id = employee.person_id;
            entity.position_id = employee.position_id;
            entity.department_id = employee.department_id;
            entity.disc_profile_id = employee.disc_profile_id;
            await _context.SaveChangesAsync();
            return MapToDto(entity);
        }


        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.employees.FindAsync(id);
            if (entity == null)
                return false;
            _context.employees.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
        private static employee MapToDto(employee e) => new()
        {
            id = e.id,
            email = e.email,
            phone = e.phone,
            company_id = e.company_id,
            person_id = e.person_id,
            position_id = e.position_id,
            department_id = e.department_id,
            disc_profile_id = e.disc_profile_id
        };

        private static employee MapToEntity(employee employee) => new()
        {
            id = employee.id,
            email = employee.email,
            phone = employee.phone,
            company_id = employee.company_id,
            person_id = employee.person_id,
            position_id = employee.position_id,
            department_id = employee.department_id,
            disc_profile_id = employee.disc_profile_id
        };
    }
}

