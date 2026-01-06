using System.Threading.Tasks;
using DiscProfilesApi.Services.GraphServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace DiscProfilesApi.Controllers.Graph
{
    [ApiController]
    [Route("api/graph/[controller]")]
    public class GraphDiscProfileController : ControllerBase
    {
        private readonly GraphDiscProfileService _graphDiscProfileService;

        public GraphDiscProfileController(GraphDiscProfileService graphDiscProfileService)
        {
            _graphDiscProfileService = graphDiscProfileService;
        }

        // GET
        [HttpGet("employees/{id:int}")]
        public async Task<IActionResult> GetEmployeesWithDiscProfile(int id)
        {
            var data = await _graphDiscProfileService.GetEmployeesWithDiscProfileAsync(id);
            return Ok(data);
        }

        [HttpGet("{id:int}/projects")]
        public async Task<IActionResult> GetProjectsWithDiscProfile(int id)
        {
            var data = await _graphDiscProfileService.GetProjectsWithDiscProfileAsync(id);
            return Ok(data);
        }

        // POST
        [HttpPost("{id:int}/sync-from-sql")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> SyncDiscProfileFromSql(int id)
        {
            await _graphDiscProfileService.MirrorDiscProfileFromSqlAsync(id);
            return NoContent();
        }

        // PUT
        [HttpPut("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateDiscProfile(int id, [FromBody] UpdateDiscProfileGraphDto dto)
        {
            var success = await _graphDiscProfileService.UpdateDiscProfileNodeAsync(id, dto.Name, dto.Color, dto.Description);
            return success ? NoContent() : NotFound();
        }

        // DELETE
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteDiscProfile(int id)
        {
            var success = await _graphDiscProfileService.DeleteDiscProfileNodeAsync(id);
            return success ? NoContent() : NotFound();
        }
    }

    public class UpdateDiscProfileGraphDto
    {
        public string? Name { get; set; }
        public string? Color { get; set; }
        public string? Description { get; set; }
    }
}