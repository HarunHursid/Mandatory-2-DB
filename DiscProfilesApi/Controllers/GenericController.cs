using DiscProfilesApi.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DiscProfilesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenericController<TEntity, TDto> : ControllerBase 
        where TEntity : class 
        where TDto : class
    {
        protected readonly IGenericService<TEntity, TDto> _service;

        public GenericController(IGenericService<TEntity, TDto> service)
        {
            _service = service;
        }

        [HttpGet]
        public virtual async Task<ActionResult<IEnumerable<TDto>>> GetAll()
        {
            var items = await _service.GetAllAsync();
            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public virtual async Task<ActionResult<TDto>> GetById(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            return dto == null ? NotFound() : Ok(dto);
        }

        [HttpPost]
        public virtual async Task<ActionResult<TDto>> Create(TDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = ((dynamic)created).id }, created);
        }

        [HttpPut("{id:int}")]
        public virtual async Task<ActionResult<TDto>> Update(int id, TDto dto)
        {
            if (((dynamic)dto).id != 0 && ((dynamic)dto).id != id)
                return BadRequest("Mismatched id");

            var updated = await _service.UpdateAsync(id, dto);
            return updated == null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id:int}")]
        public virtual async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}