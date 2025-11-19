using DiscProfilesApi.DTOs;
using DiscProfilesApi.Interfaces;
using DiscProfilesApi.Models;

namespace DiscProfilesApi.Controllers
{
    public class DepartmentController : GenericController<department, DepartmentDTO>
    {
        public DepartmentController(IGenericService<department, DepartmentDTO> service) 
            : base(service)
        {
        }
    }
}