using DiscProfilesApi.DTOs;
using DiscProfilesApi.Interfaces;
using DiscProfilesApi.Models;

namespace DiscProfilesApi.Controllers.SQL
{
    public class ProjectsDiscProfileController : GenericController<projects_disc_profile, ProjectsDiscProfileDTO>
    {
        public ProjectsDiscProfileController(IGenericService<projects_disc_profile, ProjectsDiscProfileDTO> service) 
            : base(service)
        {
        }
    }
}