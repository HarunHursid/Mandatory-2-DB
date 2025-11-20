using DiscProfilesApi.DTOs;

namespace DiscProfilesApi.Interfaces
{
    public interface IEmployee_PersonDomainInterface
    {
        Task<EmployeeDTO> CreateEmployeeWithPersonAsync(CreateEmployee_PersonRequestDto dto);
    }
}
