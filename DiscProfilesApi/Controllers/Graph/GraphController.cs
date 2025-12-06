using System.Threading.Tasks;
using DiscProfilesApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace DiscProfilesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GraphController : ControllerBase
    {
        private readonly GraphEmployeeService _graphEmployeeService;

        public GraphController(GraphEmployeeService graphEmployeeService)
        {
            _graphEmployeeService = graphEmployeeService;
        }

        // 1) Ren graph
        // GET: api/graph/employees-with-company
        [HttpGet("employees-with-company")]
        public async Task<IActionResult> GetEmployeesWithCompany()
        {
            var data = await _graphEmployeeService.GetEmployeesWithCompanyAsync();
            return Ok(data);
        }

        // 2) Ren graph
        // GET: api/graph/employee/{id}/colleagues
        [HttpGet("employee/{id:int}/colleagues")]
        public async Task<IActionResult> GetColleagues(int id)
        {
            var data = await _graphEmployeeService.GetColleaguesInSameCompanyAsync(id);
            return Ok(data);
        }

        // REN GRAPH: projekter for en employee
        // GET: api/graph/employee/{id}/projects
        [HttpGet("employee/{id:int}/projects")]
        public async Task<IActionResult> GetEmployeeProjects(int id)
        {
            var projects = await _graphEmployeeService.GetProjectsForEmployeeAsync(id);
            return Ok(projects);
        }

        // REN GRAPH: tasks for en employee (via projekter)
        // GET: api/graph/employee/{id:int}/tasks
        [HttpGet("employee/{id:int}/tasks")]
        public async Task<IActionResult> GetEmployeeTasks(int id)
        {
            var tasks = await _graphEmployeeService.GetTasksForEmployeeAsync(id);
            return Ok(tasks);
        }


        // 3) SQL + graph
        // GET: api/graph/employee/{id}/overview
        [HttpGet("employee/{id:int}/overview")]
        public async Task<IActionResult> GetEmployeeOverview(int id)
        {
            var overview = await _graphEmployeeService.GetEmployeeOverviewAsync(id);
            if (overview == null) return NotFound();
            return Ok(overview);
        }

        // 4) Write i graph
        // POST: api/graph/employees
        [HttpPost("employees")]
        public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeGraphDto dto)
        {
            await _graphEmployeeService.CreateEmployeeNodeAsync(dto.Id, dto.Email);
            return NoContent();
        }

        // POST: api/graph/employee/{id}/sync-from-sql
        [HttpPost("employee/{id:int}/sync-from-sql")]
        public async Task<IActionResult> SyncEmployeeFromSql(int id)
        {
            await _graphEmployeeService.MirrorEmployeeFromSqlAsync(id);
            return NoContent();
        }

    }

    // DTO-klasse
    public class CreateEmployeeGraphDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}
