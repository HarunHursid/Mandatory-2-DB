using Microsoft.AspNetCore.Mvc;
using DiscProfilesApi.DTOs;
using DiscProfilesApi.Interfaces;
using DiscProfilesApi.Models;
using DiscProfilesApi.MongoDocuments;
using Microsoft.AspNetCore.Authorization;

namespace DiscProfilesApi.Controllers.MongoControllers
{
    [ApiController]
    [Route("api/mongo/appusers")]
    [Authorize]
    public class AppUserMongoController : ControllerBase
    {
        private readonly IGenericMongoService<AppUserDocument, AppUserDTO> _service;

        public AppUserMongoController(
            IGenericMongoService<AppUserDocument, AppUserDTO> service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUserDTO>>> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<AppUserDTO>> GetById(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<AppUserDTO>> Create([FromBody] AppUserDTO dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.id }, created);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(int id, [FromBody] AppUserDTO dto)
        {
            var success = await _service.UpdateAsync(id, dto);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}