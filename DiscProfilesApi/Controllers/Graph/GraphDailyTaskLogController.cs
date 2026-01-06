using System.Threading.Tasks;
using DiscProfilesApi.Services.GraphServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace DiscProfilesApi.Controllers.Graph
{
    [ApiController]
    [Route("api/graph/[controller]")]
    public class GraphDailyTaskLogController : ControllerBase
    {
        private readonly GraphDailyTaskLogService _graphDailyTaskLogService;

        public GraphDailyTaskLogController(GraphDailyTaskLogService graphDailyTaskLogService)
        {
            _graphDailyTaskLogService = graphDailyTaskLogService;
        }

        // GET
        [HttpGet("task/{id:int}")]
        public async Task<IActionResult> GetLogsForTask(int id)
        {
            var data = await _graphDailyTaskLogService.GetLogsForTaskAsync(id);
            return Ok(data);
        }

        // POST
        [HttpPost("{id:int}/sync-from-sql")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> SyncDailyTaskLogFromSql(int id)
        {
            await _graphDailyTaskLogService.MirrorDailyTaskLogFromSqlAsync(id);
            return NoContent();
        }

        // PUT
        [HttpPut("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateDailyTaskLog(int id, [FromBody] UpdateDailyTaskLogGraphDto dto)
        {
            var success = await _graphDailyTaskLogService.UpdateDailyTaskLogNodeAsync(id, dto.TimeToComplete);
            return success ? NoContent() : NotFound();
        }

        // DELETE
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteDailyTaskLog(int id)
        {
            var success = await _graphDailyTaskLogService.DeleteDailyTaskLogNodeAsync(id);
            return success ? NoContent() : NotFound();
        }
    }

    public class UpdateDailyTaskLogGraphDto
    {
        public string? TimeToComplete { get; set; }
    }
}