using DiscProfilesApi.DTOs;
using DiscProfilesApi.Interfaces;
using DiscProfilesApi.Models;

namespace DiscProfilesApi.Controllers
{
    public class DiscProfileController : GenericController<disc_profile, DiscProfileDTO>
    {
        public DiscProfileController(IGenericService<disc_profile, DiscProfileDTO> service) 
            : base(service)
        {
        }
    }
}