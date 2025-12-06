using DiscProfilesApi.DTOs;
using DiscProfilesApi.Interfaces;
using DiscProfilesApi.Models;

namespace DiscProfilesApi.Controllers.SQL
{
    public class DepartmentController : GenericController<department, DepartmentDTO>
    {
        public DepartmentController(IGenericService<department, DepartmentDTO> service) 
            : base(service)
        {
        }
    }
}