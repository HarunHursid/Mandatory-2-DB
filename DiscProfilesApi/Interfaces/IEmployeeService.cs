using DiscProfilesApi;
using DiscProfilesApi.Models;



namespace DiscProfilesApi.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<employee>> GetAllAsync();
        Task<employee?> GetByIdAsync(int id);
        Task<employee> CreateAsync(employee employee);
        Task<employee> UpdateAsync(employee employee);
        Task<bool> DeleteAsync(int id);
    }
}
