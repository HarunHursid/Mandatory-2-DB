using DiscProfilesApi.DTOs;
using DiscProfilesApi.Interfaces;
using DiscProfilesApi.Models;

namespace DiscProfilesApi.Controllers.SQL
{
    public class PositionController : GenericController<position, PositionDTO>
    {
        public PositionController(IGenericService<position, PositionDTO> service) 
            : base(service)
        {
        }
    }
}