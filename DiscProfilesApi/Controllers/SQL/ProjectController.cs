using DiscProfilesApi.DTOs;
using DiscProfilesApi.Interfaces;
using DiscProfilesApi.Models;

namespace DiscProfilesApi.Controllers.SQL
{
    public class ProjectController : GenericController<project, ProjectDTO>
    {
        public ProjectController(IGenericService<project, ProjectDTO> service) 
            : base(service)
        {
        }
    }
}