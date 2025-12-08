using DiscProfilesApi.DTOs;
using DiscProfilesApi.Interfaces;
using DiscProfilesApi.Models;

namespace DiscProfilesApi.Controllers.SQL
{
    public class PersonController : GenericController<person, PersonDTO>
    {
        public PersonController(IGenericService<person, PersonDTO> service) 
            : base(service)
        {
        }
    }
}