using DiscProfilesApi.DTOs;
using DiscProfilesApi.Interfaces;
using DiscProfilesApi.Models;

namespace DiscProfilesApi.Controllers
{
    public class TaskEvaluationController : GenericController<task_evaluation, TaskEvaluationDTO>
    {
        public TaskEvaluationController(IGenericService<task_evaluation, TaskEvaluationDTO> service) 
            : base(service)
        {
        }
    }
}