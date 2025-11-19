using DiscProfilesApi;
using DiscProfilesApi.DTOs;
using DiscProfilesApi.Models;



namespace DiscProfilesApi.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeDTO>> GetAllAsync();
        Task<EmployeeDTO?> GetByIdAsync(int id);
        Task<EmployeeDTO> CreateAsync(EmployeeDTO dto);
        Task<EmployeeDTO?> UpdateAsync(int id, EmployeeDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
