using DiscProfilesApi.DTOs;
using DiscProfilesApi.Interfaces;
using DiscProfilesApi.Models;

namespace DiscProfilesApi.Controllers
{
    public class DailyTaskLogController : GenericController<daily_task_log, DailyTaskLogDTO>
    {
        public DailyTaskLogController(IGenericService<daily_task_log, DailyTaskLogDTO> service) 
            : base(service)
        {
        }
    }
}