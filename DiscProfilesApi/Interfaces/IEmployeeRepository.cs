using DiscProfilesApi.Models;

namespace DiscProfilesApi.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<employee>> GetAllAsync();
        Task<employee?> GetByIdAsync(int id);
        Task<employee> AddAsync(employee entity);
        Task<employee?> UpdateAsync(employee entity);
        Task<bool> DeleteAsync(int id);
    }
}
