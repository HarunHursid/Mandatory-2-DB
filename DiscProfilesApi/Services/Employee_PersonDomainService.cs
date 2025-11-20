using DiscProfilesApi.DTOs;
using DiscProfilesApi.Interfaces;
using DiscProfilesApi.Models;

namespace DiscProfilesApi.Services
{
    public class Employee_PersonDomainService : IEmployee_PersonDomainInterface
    {
        private readonly DiscProfilesContext _context;
        private readonly IGenericRepository<person> _personRepo;
        private readonly IGenericRepository<employee> _employeeRepo;

        public Employee_PersonDomainService(
            DiscProfilesContext context,
            IGenericRepository<person> personRepo,
            IGenericRepository<employee> employeeRepo)
        {
            _context = context;
            _personRepo = personRepo;
            _employeeRepo = employeeRepo;
        }

        public async Task<EmployeeDTO> CreateEmployeeWithPersonAsync(CreateEmployee_PersonRequestDto dto)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1) Person via generic repo
                var person = new person
                {
                    private_email = dto.PrivateEmail,
                    private_phone = dto.PrivatePhone,
                    cpr = dto.Cpr,
                    first_name = dto.FirstName,
                    last_name = dto.LastName,
                    experience = dto.Experience,
                    EducationID = dto.EducationId
                };

                person = await _personRepo.AddAsync(person);

                // 2) Employee via generic repo
                var employee = new employee
                {
                    email = dto.WorkEmail,
                    phone = dto.WorkPhone,
                    company_id = dto.CompanyId,
                    person_id = person.id,
                    department_id = dto.DepartmentId,
                    position_id = dto.PositionId,
                    disc_profile_id = dto.DiscProfileId
                };

                employee = await _employeeRepo.AddAsync(employee);

                await transaction.CommitAsync();

                return new EmployeeDTO
                {
                    id = employee.id,
                    email = employee.email,
                    phone = employee.phone,
                    company_id = employee.company_id,
                    person_id = employee.person_id,
                    department_id = employee.department_id,
                    position_id = employee.position_id,
                    disc_profile_id = employee.disc_profile_id
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
