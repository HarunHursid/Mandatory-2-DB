using DiscProfilesApi.DTOs;
using DiscProfilesApi.Interfaces;
using DiscProfilesApi.Models;

namespace DiscProfilesApi.Controllers.SQL
{
    public class VwSocialEventsOverviewController : GenericController<vw_SocialEventsOverview, VwSocialEventsOverviewDTO>
    {
        public VwSocialEventsOverviewController(IGenericService<vw_SocialEventsOverview, VwSocialEventsOverviewDTO> service) 
            : base(service)
        {
        }
    }
}