using System.Threading.Tasks;
using DiscProfilesApi.Services.GraphServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace DiscProfilesApi.Controllers.Graph
{
    [ApiController]
    [Route("api/graph/[controller]")]
    public class GraphProjectController : ControllerBase
    {
        private readonly GraphProjectService _graphProjectService;

        public GraphProjectController(GraphProjectService graphProjectService)
        {
            _graphProjectService = graphProjectService;
        }

        // GET
        [HttpGet("{id:int}/tasks")]
        public async Task<IActionResult> GetTasksInProject(int id)
        {
            var data = await _graphProjectService.GetTasksInProjectAsync(id);
            return Ok(data);
        }

        [HttpGet("{id:int}/employees")]
        public async Task<IActionResult> GetEmployeesOnProject(int id)
        {
            var data = await _graphProjectService.GetEmployeesOnProjectAsync(id);
            return Ok(data);
        }

        // POST
        [HttpPost("{id:int}/sync-from-sql")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> SyncProjectFromSql(int id)
        {
            await _graphProjectService.MirrorProjectFromSqlAsync(id);
            return NoContent();
        }

        // PUT
        [HttpPut("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateProject(int id, [FromBody] UpdateProjectGraphDto dto)
        {
            var success = await _graphProjectService.UpdateProjectNodeAsync(id, dto.Name, dto.Description, dto.Deadline, dto.Completed, dto.NumberOfEmployees);
            return success ? NoContent() : NotFound();
        }

        // DELETE
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var success = await _graphProjectService.DeleteProjectNodeAsync(id);
            return success ? NoContent() : NotFound();
        }
    }

    public class UpdateProjectGraphDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Deadline { get; set; }
        public string? Completed { get; set; }
        public int? NumberOfEmployees { get; set; }
    }
}