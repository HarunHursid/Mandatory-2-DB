using DiscProfilesApi.DTOs;
using DiscProfilesApi.Interfaces;
using DiscProfilesApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DiscProfilesApi.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repo;

        public EmployeeService(IEmployeeRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<EmployeeDTO>> GetAllAsync()
        {
            var entities = await _repo.GetAllAsync();
            return entities.Select(MapToDto);
        }

        public async Task<EmployeeDTO?> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : MapToDto(entity);
        }

        public async Task<EmployeeDTO> CreateAsync(EmployeeDTO dto)
        {
            var entity = new employee
            {
                email = dto.email,
                phone = dto.phone,
                company_id = dto.company_id,
                person_id = dto.person_id,
                department_id = dto.department_id,
                position_id = dto.position_id,
                disc_profile_id = dto.disc_profile_id
            };
            var created = await _repo.AddAsync(entity);
            return MapToDto(created);
        }

        public async Task<EmployeeDTO?> UpdateAsync(int id, EmployeeDTO dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return null;

            existing.email = dto.email;
            existing.phone = dto.phone;
            existing.company_id = dto.company_id;
            existing.person_id = dto.person_id;
            existing.department_id = dto.department_id;
            existing.position_id = dto.position_id;
            existing.disc_profile_id = dto.disc_profile_id;

            var updated = await _repo.UpdateAsync(existing);
            return updated == null ? null : MapToDto(updated);
        }

        public Task<bool> DeleteAsync(int id) => _repo.DeleteAsync(id);

        private static EmployeeDTO MapToDto(employee e) => new()
        {
            id = e.id,
            email = e.email,
            phone = e.phone,
            company_id = e.company_id,
            person_id = e.person_id,
            department_id = e.department_id,
            position_id = e.position_id,
            disc_profile_id = e.disc_profile_id
        };
    }
}

