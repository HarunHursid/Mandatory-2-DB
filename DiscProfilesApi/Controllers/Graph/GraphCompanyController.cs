using System.Threading.Tasks;
using DiscProfilesApi.Services.GraphServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace DiscProfilesApi.Controllers.Graph
{
    [ApiController]
    [Route("api/graph/[controller]")]
    public class GraphCompanyController : ControllerBase
    {
        private readonly GraphCompanyService _graphCompanyService;

        public GraphCompanyController(GraphCompanyService graphCompanyService)
        {
            _graphCompanyService = graphCompanyService;
        }

        // GET
        [HttpGet("with-employee-count")]
        public async Task<IActionResult> GetCompaniesWithEmployeeCount()
        {
            var data = await _graphCompanyService.GetCompaniesWithEmployeeCountAsync();
            return Ok(data);
        }

        [HttpGet("{id:int}/departments")]
        public async Task<IActionResult> GetDepartmentsInCompany(int id)
        {
            var data = await _graphCompanyService.GetDepartmentsInCompanyAsync(id);
            return Ok(data);
        }

        // POST
        [HttpPost("{id:int}/sync-from-sql")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> SyncCompanyFromSql(int id)
        {
            await _graphCompanyService.MirrorCompanyFromSqlAsync(id);
            return NoContent();
        }

        // PUT
        [HttpPut("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateCompany(int id, [FromBody] UpdateCompanyGraphDto dto)
        {
            var success = await _graphCompanyService.UpdateCompanyNodeAsync(id, dto.Name, dto.Location, dto.BusinessField);
            return success ? NoContent() : NotFound();
        }

        // DELETE
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            var success = await _graphCompanyService.DeleteCompanyNodeAsync(id);
            return success ? NoContent() : NotFound();
        }
    }

    public class UpdateCompanyGraphDto
    {
        public string? Name { get; set; }
        public string? Location { get; set; }
        public string? BusinessField { get; set; }
    }
}
