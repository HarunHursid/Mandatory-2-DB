using DiscProfilesApi.DTOs;
using DiscProfilesApi.Interfaces;
using DiscProfilesApi.Models;

namespace DiscProfilesApi.Controllers
{
    public class SocialEventController : GenericController<social_event, SocialEventDTO>
    {
        public SocialEventController(IGenericService<social_event, SocialEventDTO> service) 
            : base(service)
        {
        }
    }
}