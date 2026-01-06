using DiscProfilesApi.Models;
using DiscProfilesApi.Services.GraphServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiscProfilesApi.Controllers.SQL
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminAssignmentsController : ControllerBase
    {
        private readonly DiscProfilesContext _context;
        private readonly GraphEmployeeService _graphEmployeeService;

        public AdminAssignmentsController(
            DiscProfilesContext context,
            GraphEmployeeService graphEmployeeService)
        {
            _context = context;
            _graphEmployeeService = graphEmployeeService;
        }

        // POST: api/AdminAssignments/assign-employee-project?employeeId=12&projectId=6
        [HttpPost("assign-employee-project")]
        public async Task<IActionResult> AssignEmployeeToProject(int employeeId, int projectId)
        {
            var employee = await _context.employees
                .Include(e => e.projects)
                .FirstOrDefaultAsync(e => e.id == employeeId);

            var project = await _context.projects.FindAsync(projectId);

            if (employee == null || project == null)
                return NotFound();

            if (!employee.projects.Any(p => p.id == projectId))
            {
                employee.projects.Add(project);
                await _context.SaveChangesAsync();
            }

            await _graphEmployeeService.MirrorEmployeeFromSqlAsync(employeeId);
            return NoContent();
        }

        // POST: api/AdminAssignments/assign-employee-task?employeeId=12&taskId=21
        [HttpPost("assign-employee-task")]
        public async Task<IActionResult> AssignEmployeeToTask(int employeeId, int taskId)
        {
            var employee = await _context.employees
                .Include(e => e.tasks)
                .FirstOrDefaultAsync(e => e.id == employeeId);

            var task = await _context.tasks
                .Include(t => t.project)
                .FirstOrDefaultAsync(t => t.id == taskId);

            if (employee == null || task == null)
                return NotFound();

            if (!employee.tasks.Any(t => t.id == taskId))
            {
                employee.tasks.Add(task);
                await _context.SaveChangesAsync();
            }

            await _graphEmployeeService.MirrorEmployeeFromSqlAsync(employeeId);
            return NoContent();
        }
    }
}
