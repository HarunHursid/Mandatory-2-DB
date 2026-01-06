using DiscProfilesApi.DTOs;
using DiscProfilesApi.Interfaces;
using DiscProfilesApi.Models;
using DiscProfilesApi.Services;
using DiscProfilesApi.Services.GraphServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiscProfilesApi.Controllers.SQL;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "admin")]
public class AppUserController : ControllerBase
{
    private readonly IGenericService<AppUser, AppUserDTO> _service;
    private readonly DiscProfilesContext _context;
    private readonly IPasswordHashService _passwordHashService;
    private readonly GraphEmployeeService _graphEmployeeService;

    public AppUserController(
        IGenericService<AppUser, AppUserDTO> service,
        DiscProfilesContext context,
        IPasswordHashService passwordHashService,
        GraphEmployeeService graphEmployeeService)
    {
        _service = service;
        _context = context;
        _passwordHashService = passwordHashService;
        _graphEmployeeService = graphEmployeeService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppUserDTO>>> GetAll()
    {
        var users = await _service.GetAllAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AppUserDTO>> GetById(int id)
    {
        var user = await _service.GetByIdAsync(id);
        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<AppUserDTO>> Create([FromBody] CreateAppUserDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Tjek om email allerede findes i AppUsers
        var exists = await _context.AppUsers
            .AsNoTracking()
            .AnyAsync(u => u.Email == dto.email);

        if (exists)
            return Conflict(new { message = "User with this email already exists" });

        // company_id MÅ ikke være 0
        if (dto.company_id <= 0)
            return BadRequest(new { message = "company_id is required and must be > 0" });

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // 1) PERSON
            var person = new person
            {
                first_name = dto.first_name,
                last_name = dto.last_name,
                private_email = dto.private_email,
                private_phone = dto.private_phone,
                cpr = dto.cpr,
                experience = dto.experience,
                EducationID = dto.education_id
            };

            _context.persons.Add(person);
            await _context.SaveChangesAsync();   // person.id

            // 2) EMPLOYEE
            var employee = new employee
            {
                email = dto.email,                   // arbejds-mail = login-mail
                phone = dto.employee_phone,
                company_id = dto.company_id,
                person_id = person.id,
                department_id = dto.department_id,
                position_id = dto.position_id,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                LastLogin = DateTime.UtcNow
            };

            _context.employees.Add(employee);
            await _context.SaveChangesAsync();      // employee.id

            // 3) APPUSER
            var hash = _passwordHashService.HashPassword(dto.password);

            var user = new AppUser
            {
                Email = dto.email,
                PasswordHash = hash,
                Role = dto.role.ToLowerInvariant(),
                EmployeeId = employee.id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.AppUsers.Add(user);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            // 4) Spejl employee til Neo4j (graph)
            try
            {
                await _graphEmployeeService.MirrorEmployeeFromSqlAsync(employee.id);
            }
            catch
            {
                // Hvis graph-fejl, lader vi SQL stå som den er.
                // Evt. TODO: log fejlen.
            }

            var resultDto = new AppUserDTO
            {
                id = user.Id,
                email = user.Email,
                role = user.Role,
                is_active = user.IsActive,
                employee_id = user.EmployeeId,
                created_at = user.CreatedAt,
                last_login = user.LastLogin
            };

            return CreatedAtAction(nameof(GetById), new { resultDto.id }, resultDto);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            var innerMessage = ex.InnerException?.Message ?? ex.Message;

            return StatusCode(500, new
            {
                message = "Kunne ikke oprette user + employee + person.",
                error = innerMessage
            });
        }
    }

    // DELETE: api/AppUser/{id}
    // Soft delete: vi sætter IsActive = false i stedet for at slette rækken
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _context.AppUsers.FindAsync(id);
        if (user == null)
            return NotFound();

        user.IsActive = false;
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
