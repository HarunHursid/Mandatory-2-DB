<<<<<<< HEAD
﻿using System.Threading.Tasks;
using DiscProfilesApi.Services;
using DiscProfilesApi.Services.GraphServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
=======
﻿using DiscProfilesApi.Services;
using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
>>>>>>> addf99358a1c048c95b095551ddd0c4fcbf9d760

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

        // ========== GET (READ) ==========

        // GET: api/graph/employees-with-company
        [HttpGet("employees-with-company")]
        public async Task<IActionResult> GetEmployeesWithCompany()
        {
            var data = await _graphEmployeeService.GetEmployeesWithCompanyAsync();
            return Ok(data);
        }

        // GET: api/graph/employee/{id}/colleagues
        [HttpGet("employee/{id:int}/colleagues")]
        public async Task<IActionResult> GetColleagues(int id)
        {
            var data = await _graphEmployeeService.GetColleaguesInSameCompanyAsync(id);
            return Ok(data);
        }

        // GET: api/graph/employee/{id}/projects
        [HttpGet("employee/{id:int}/projects")]
        public async Task<IActionResult> GetEmployeeProjects(int id)
        {
            var projects = await _graphEmployeeService.GetProjectsForEmployeeAsync(id);
            return Ok(projects);
        }

        // GET: api/graph/employee/{id}/tasks
        [HttpGet("employee/{id:int}/tasks")]
        public async Task<IActionResult> GetEmployeeTasks(int id)
        {
            var tasks = await _graphEmployeeService.GetTasksForEmployeeAsync(id);
            return Ok(tasks);
        }

        // GET: api/graph/employee/{id}/overview
        [HttpGet("employee/{id:int}/overview")]
        public async Task<IActionResult> GetEmployeeOverview(int id)
        {
            var overview = await _graphEmployeeService.GetEmployeeOverviewAsync(id);
            if (overview == null) return NotFound();
            return Ok(overview);
        }

        // ========== POST (CREATE) ==========

        // POST: api/graph/employees
        [HttpPost("employees")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeGraphDto dto)
        {
            await _graphEmployeeService.CreateEmployeeNodeAsync(dto.Id, dto.Email);
            return CreatedAtAction(nameof(GetEmployeeProjects), new { id = dto.Id });
        }

        // POST: api/graph/employee/{id}/sync-from-sql
        [HttpPost("employee/{id:int}/sync-from-sql")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> SyncEmployeeFromSql(int id)
        {
            await _graphEmployeeService.MirrorEmployeeFromSqlAsync(id);
            return NoContent();
        }

<<<<<<< HEAD
        // ========== PUT (UPDATE) ==========

        // PUT: api/graph/employee/{id}
        [HttpPut("employee/{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] UpdateEmployeeGraphDto dto)
        {
            var success = await _graphEmployeeService.UpdateEmployeeNodeAsync(id, dto.Email, dto.Phone);
            return success ? NoContent() : NotFound();
        }

        // ========== DELETE ==========

        // DELETE: api/graph/employee/{id}
        [HttpDelete("employee/{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var success = await _graphEmployeeService.DeleteEmployeeNodeAsync(id);
            return success ? NoContent() : NotFound();
        }
=======
        [HttpGet("node/{label}/{id:int}")]
        public async Task<IActionResult> GetNodeByLabelAndId(string label, int id)
        {
            // valider label her (eller i service) - jeg gør det her for at returnere BadRequest pænt
            if (!Regex.IsMatch(label, "^[A-Za-z_][A-Za-z0-9_]*$"))
                return BadRequest("Invalid label");

            var props = await _graphEmployeeService.GetNodeByLabelAndIdAsync(label, id);
            if (props is null) return NotFound();

            return Ok(props);
        }

>>>>>>> addf99358a1c048c95b095551ddd0c4fcbf9d760
    }

    // DTO-klasser
    public class CreateEmployeeGraphDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
    }

    public class UpdateEmployeeGraphDto
    {
        public string? Email { get; set; }
        public string? Phone { get; set; }
    }
}
