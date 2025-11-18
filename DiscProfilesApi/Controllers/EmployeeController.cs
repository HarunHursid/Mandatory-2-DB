using DiscProfilesApi.Interfaces;
using DiscProfilesApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DiscProfilesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase 
    {
        private readonly IEmployeeService _service;

        public EmployeeController(IEmployeeService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var entity = await _service.GetByIdAsync(id);
            return entity == null ? NotFound() : Ok(entity);
        }

        [HttpPost]
        public async Task<IActionResult> Create(employee employee)
        {
            var created = await _service.CreateAsync(employee);
            return CreatedAtAction(nameof(GetById), new { Id = created.id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id ,employee employee)
        {
            if (id != employee.id)
                return BadRequest("Mismatched id");

            var existing = await _service.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            var updated = await _service.UpdateAsync(employee);
            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }

    }
}
