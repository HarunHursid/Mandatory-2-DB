using DiscProfilesApi.Controllers.SQL;
using DiscProfilesApi.DTOs;
using DiscProfilesApi.Interfaces;
using DiscProfilesApi.Models;

namespace DiscProfilesApi.Controllers
{
    public class TaskController : GenericController<task, TaskDTO>
    {
        public TaskController(IGenericService<task, TaskDTO> service) 
            : base(service)
        {
        }
    }
}