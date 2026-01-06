using System.Threading.Tasks;
using DiscProfilesApi.Services.GraphServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace DiscProfilesApi.Controllers.Graph
{
    [ApiController]
    [Route("api/graph/[controller]")]
    public class GraphTaskEvaluationController : ControllerBase
    {
        private readonly GraphTaskEvaluationService _graphTaskEvaluationService;

        public GraphTaskEvaluationController(GraphTaskEvaluationService graphTaskEvaluationService)
        {
            _graphTaskEvaluationService = graphTaskEvaluationService;
        }

        // GET
        [HttpGet("task/{id:int}")]
        public async Task<IActionResult> GetEvaluationsForTask(int id)
        {
            var data = await _graphTaskEvaluationService.GetEvaluationsForTaskAsync(id);
            return Ok(data);
        }

        // POST
        [HttpPost("{id:int}/sync-from-sql")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> SyncTaskEvaluationFromSql(int id)
        {
            await _graphTaskEvaluationService.MirrorTaskEvaluationFromSqlAsync(id);
            return NoContent();
        }

        // PUT
        [HttpPut("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateTaskEvaluation(int id, [FromBody] UpdateTaskEvaluationGraphDto dto)
        {
            var success = await _graphTaskEvaluationService.UpdateTaskEvaluationNodeAsync(id, dto.Description, dto.DifficultyRange);
            return success ? NoContent() : NotFound();
        }

        // DELETE
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteTaskEvaluation(int id)
        {
            var success = await _graphTaskEvaluationService.DeleteTaskEvaluationNodeAsync(id);
            return success ? NoContent() : NotFound();
        }
    }

    public class UpdateTaskEvaluationGraphDto
    {
        public string? Description { get; set; }
        public int? DifficultyRange { get; set; }
    }
}