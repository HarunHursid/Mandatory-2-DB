using System.Threading.Tasks;
using DiscProfilesApi.Services.GraphServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace DiscProfilesApi.Controllers.Graph
{
    [ApiController]
    [Route("api/graph/[controller]")]
    public class GraphTaskController : ControllerBase
    {
        private readonly GraphTaskService _graphTaskService;

        public GraphTaskController(GraphTaskService graphTaskService)
        {
            _graphTaskService = graphTaskService;
        }

        // REN GRAPH: alle medarbejdere tildelt en task
        // GET: api/graph/task/{id}/employees
        [HttpGet("{id:int}/employees")]
        public async Task<IActionResult> GetEmployeesAssignedToTask(int id)
        {
            var data = await _graphTaskService.GetEmployeesAssignedToTaskAsync(id);
            return Ok(data);
        }

        // POST: api/graph/task/{id}/sync-from-sql
        [HttpPost("{id:int}/sync-from-sql")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> SyncTaskFromSql(int id)
        {
            await _graphTaskService.MirrorTaskFromSqlAsync(id);
            return NoContent();
        }

        // PUT
        [HttpPut("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskGraphDto dto)
        {
            var success = await _graphTaskService.UpdateTaskNodeAsync(id, dto.Name, dto.Completed);
            return success ? NoContent() : NotFound();
        }

        // DELETE
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var success = await _graphTaskService.DeleteTaskNodeAsync(id);
            return success ? NoContent() : NotFound();
        }
    }

    public class UpdateTaskGraphDto
    {
        public string? Name { get; set; }
        public bool? Completed { get; set; }
    }
}