using System.Threading.Tasks;
using DiscProfilesApi.Services.GraphServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace DiscProfilesApi.Controllers.Graph
{
    [ApiController]
    [Route("api/graph/[controller]")]
    public class GraphPositionController : ControllerBase
    {
        private readonly GraphPositionService _graphPositionService;

        public GraphPositionController(GraphPositionService graphPositionService)
        {
            _graphPositionService = graphPositionService;
        }

        // REN GRAPH: alle employees med en stilling
        // GET: api/graph/position/{id}/employees
        [HttpGet("{id:int}/employees")]
        public async Task<IActionResult> GetEmployeesWithPosition(int id)
        {
            var data = await _graphPositionService.GetEmployeesWithPositionAsync(id);
            return Ok(data);
        }

        // POST: api/graph/position/{id}/sync-from-sql
        [HttpPost("{id:int}/sync-from-sql")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> SyncPositionFromSql(int id)
        {
            await _graphPositionService.MirrorPositionFromSqlAsync(id);
            return NoContent();
        }

        // PUT: api/graph/position/{id}
        [HttpPut("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdatePosition(int id, [FromBody] UpdatePositionGraphDto dto)
        {
            var success = await _graphPositionService.UpdatePositionNodeAsync(id, dto.Name);
            return success ? NoContent() : NotFound();
        }

        // DELETE: api/graph/position/{id}
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeletePosition(int id)
        {
            var success = await _graphPositionService.DeletePositionNodeAsync(id);
            return success ? NoContent() : NotFound();
        }
    }

    public class UpdatePositionGraphDto
    {
        public string? Name { get; set; }
    }
}