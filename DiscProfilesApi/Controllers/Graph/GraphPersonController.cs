using System.Threading.Tasks;
using DiscProfilesApi.Services.GraphServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace DiscProfilesApi.Controllers.Graph
{
    [ApiController]
    [Route("api/graph/[controller]")]
    public class GraphPersonController : ControllerBase
    {
        private readonly GraphPersonService _graphPersonService;

        public GraphPersonController(GraphPersonService graphPersonService)
        {
            _graphPersonService = graphPersonService;
        }

        // GET
        [HttpGet("{id:int}/employees")]
        public async Task<IActionResult> GetEmployeesForPerson(int id)
        {
            var data = await _graphPersonService.GetEmployeesForPersonAsync(id);
            return Ok(data);
        }

        // POST
        [HttpPost("{id:int}/sync-from-sql")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> SyncPersonFromSql(int id)
        {
            await _graphPersonService.MirrorPersonFromSqlAsync(id);
            return NoContent();
        }

        // PUT
        [HttpPut("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdatePerson(int id, [FromBody] UpdatePersonGraphDto dto)
        {
            var success = await _graphPersonService.UpdatePersonNodeAsync(id, dto.FirstName, dto.LastName, dto.Experience);
            return success ? NoContent() : NotFound();
        }

        // DELETE
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeletePerson(int id)
        {
            var success = await _graphPersonService.DeletePersonNodeAsync(id);
            return success ? NoContent() : NotFound();
        }
    }

    public class UpdatePersonGraphDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int? Experience { get; set; }
    }
}