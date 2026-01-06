using System.Threading.Tasks;
using DiscProfilesApi.Services.GraphServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace DiscProfilesApi.Controllers.Graph
{
    [ApiController]
    [Route("api/graph/[controller]")]
    public class GraphDepartmentController : ControllerBase
    {
        private readonly GraphDepartmentService _graphDepartmentService;

        public GraphDepartmentController(GraphDepartmentService graphDepartmentService)
        {
            _graphDepartmentService = graphDepartmentService;
        }

        // REN GRAPH: alle medarbejdere i et departement
        // GET: api/graph/department/{id}/employees
        [HttpGet("{id:int}/employees")]
        public async Task<IActionResult> GetEmployeesInDepartment(int id)
        {
            var data = await _graphDepartmentService.GetEmployeesInDepartmentAsync(id);
            return Ok(data);
        }

        // POST: api/graph/department/{id}/sync-from-sql
        [HttpPost("{id:int}/sync-from-sql")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> SyncDepartmentFromSql(int id)
        {
            await _graphDepartmentService.MirrorDepartmentFromSqlAsync(id);
            return NoContent();
        }

        // PUT
        [HttpPut("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateDepartment(int id, [FromBody] UpdateDepartmentGraphDto dto)
        {
            var success = await _graphDepartmentService.UpdateDepartmentNodeAsync(id, dto.Name);
            return success ? NoContent() : NotFound();
        }

        // DELETE
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var success = await _graphDepartmentService.DeleteDepartmentNodeAsync(id);
            return success ? NoContent() : NotFound();
        }
    }

    public class UpdateDepartmentGraphDto
    {
        public string? Name { get; set; }
    }
}