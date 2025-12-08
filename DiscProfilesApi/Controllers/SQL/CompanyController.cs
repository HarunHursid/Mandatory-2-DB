using DiscProfilesApi.DTOs;
using DiscProfilesApi.Interfaces;
using DiscProfilesApi.Models;

namespace DiscProfilesApi.Controllers.SQL
{
    public class CompanyController : GenericController<company, CompanyDTO>
    {
        public CompanyController(IGenericService<company, CompanyDTO> service) 
            : base(service)
        {
        }
    }
}