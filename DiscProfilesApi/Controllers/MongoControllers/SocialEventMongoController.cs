using Microsoft.AspNetCore.Mvc;
using DiscProfilesApi.DTOs;
using DiscProfilesApi.Interfaces;
using DiscProfilesApi.MongoDocuments;

namespace DiscProfilesApi.Controllers.MongoControllers
{
    [ApiController]
    [Route("api/mongo/socialevents")]
    public class SocialEventMongoController : ControllerBase
    {
        private readonly IGenericMongoService<SocialEventDocument, SocialEventDTO> _service;

        public SocialEventMongoController(
            IGenericMongoService<SocialEventDocument, SocialEventDTO> service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SocialEventDTO>>> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<SocialEventDTO>> GetById(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<SocialEventDTO>> Create([FromBody] SocialEventDTO dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] SocialEventDTO dto)
        {
            var success = await _service.UpdateAsync(id, dto);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}