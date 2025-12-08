using DiscProfilesApi.DTOs;
using DiscProfilesApi.Interfaces;
using DiscProfilesApi.Models;
using DiscProfilesApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscProfilesApi.Controllers.SQL;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly DiscProfilesContext _context;
    private readonly IPasswordHashService _passwordHashService;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthController(
        DiscProfilesContext context,
        IPasswordHashService passwordHashService,
        IJwtTokenService jwtTokenService)
    {
        _context = context;
        _passwordHashService = passwordHashService;
        _jwtTokenService = jwtTokenService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _context.AppUsers
            .FirstOrDefaultAsync(u => u.Email == request.email);
            //Finder brugeren med email - hvilket vi gerne vil have den skal kunne gøre
        if (user == null || !user.IsActive)
            return Unauthorized(new { message = "Invalid email or password" });

        if (string.IsNullOrEmpty(user.PasswordHash))
            return Unauthorized(new { message = "User account misconfigured" });
        // Her verificere vi den den simple test kodeord med den gemte hash (BCrypt)
        if (!_passwordHashService.VerifyPassword(request.password, user.PasswordHash))
            return Unauthorized(new { message = "Invalid email or password" });

        user.LastLogin = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        //Generer JWT token
        var token = _jwtTokenService.GenerateToken(user.Id, user.Email, user.Role);

        return Ok(new LoginResponse
        {
            id = user.Id,
            email = user.Email,
            role = user.Role,
            token = token
        });
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<AppUserDTO>> Me()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var user = await _context.AppUsers.FindAsync(userId);
        if (user == null)
            return NotFound();

        var dto = new AppUserDTO
        {
            id = user.Id,
            email = user.Email,
            role = user.Role,
            is_active = user.IsActive,
            employee_id = user.EmployeeId,
            created_at = user.CreatedAt,
            last_login = user.LastLogin
        };

        return Ok(dto);
    }

    [HttpPost("logout")]
    [Authorize]
    public ActionResult Logout()
    {
        return Ok(new { message = "Logged out successfully" });
    }
}