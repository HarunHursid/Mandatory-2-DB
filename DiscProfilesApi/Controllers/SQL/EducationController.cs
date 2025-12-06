using DiscProfilesApi.DTOs;
using DiscProfilesApi.Interfaces;
using DiscProfilesApi.Models;

namespace DiscProfilesApi.Controllers.SQL
{
    public class EducationController : GenericController<education, EducationDTO>
    {
        public EducationController(IGenericService<education, EducationDTO> service) 
            : base(service)
        {
        }
    }
}