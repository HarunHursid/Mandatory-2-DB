using System.Threading.Tasks;
using DiscProfilesApi.Services.GraphServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace DiscProfilesApi.Controllers.Graph
{
    [ApiController]
    [Route("api/graph/[controller]")]
    public class GraphStressMeasureController : ControllerBase
    {
        private readonly GraphStressMeasureService _graphStressMeasureService;

        public GraphStressMeasureController(GraphStressMeasureService graphStressMeasureService)
        {
            _graphStressMeasureService = graphStressMeasureService;
        }

        // REN GRAPH: alle stress measures for en employee
        // GET: api/graph/stressmeasure/employee/{id}
        [HttpGet("employee/{id:int}")]
        public async Task<IActionResult> GetStressMeasuresForEmployee(int id)
        {
            var data = await _graphStressMeasureService.GetStressMeasuresForEmployeeAsync(id);
            return Ok(data);
        }

        // POST: api/graph/stressmeasure/{id}/sync-from-sql
        [HttpPost("{id:int}/sync-from-sql")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> SyncStressMeasureFromSql(int id)
        {
            await _graphStressMeasureService.MirrorStressMeasureFromSqlAsync(id);
            return NoContent();
        }

        // PUT: api/graph/stressmeasure/{id}
        [HttpPut("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateStressMeasure(int id, [FromBody] UpdateStressMeasureGraphDto dto)
        {
            var success = await _graphStressMeasureService.UpdateStressMeasureNodeAsync(id, dto.Description, dto.Measure);
            return success ? NoContent() : NotFound();
        }

        // DELETE: api/graph/stressmeasure/{id}
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteStressMeasure(int id)
        {
            var success = await _graphStressMeasureService.DeleteStressMeasureNodeAsync(id);
            return success ? NoContent() : NotFound();
        }
    }

    public class UpdateStressMeasureGraphDto
    {
        public string? Description { get; set; }
        public int? Measure { get; set; }
    }
}