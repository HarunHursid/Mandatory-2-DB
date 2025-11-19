using DiscProfilesApi.DTOs;
using DiscProfilesApi.Interfaces;
using DiscProfilesApi.Models;

namespace DiscProfilesApi.Controllers
{
    public class EmployeeController : GenericController<employee, EmployeeDTO>
    {
        public EmployeeController(IGenericService<employee, EmployeeDTO> service) 
            : base(service)
        {
        }
    }
}
