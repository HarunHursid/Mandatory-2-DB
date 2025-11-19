using DiscProfilesApi.DTOs;
using DiscProfilesApi.Interfaces;
using DiscProfilesApi.Models;

namespace DiscProfilesApi.Controllers
{
    public class StressMeasureController : GenericController<stress_measure, StressMeasureDTO>
    {
        public StressMeasureController(IGenericService<stress_measure, StressMeasureDTO> service) 
            : base(service)
        {
        }
    }
}