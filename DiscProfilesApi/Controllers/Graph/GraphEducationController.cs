using System.Threading.Tasks;
using DiscProfilesApi.Services.GraphServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace DiscProfilesApi.Controllers.Graph
{
    [ApiController]
    [Route("api/graph/[controller]")]
    public class GraphEducationController : ControllerBase
    {
        private readonly GraphEducationService _graphEducationService;

        public GraphEducationController(GraphEducationService graphEducationService)
        {
            _graphEducationService = graphEducationService;
        }

        // GET
        [HttpGet("{id:int}/persons")]
        public async Task<IActionResult> GetPersonsWithEducation(int id)
        {
            var data = await _graphEducationService.GetPersonsWithEducationAsync(id);
            return Ok(data);
        }

        // POST
        [HttpPost("{id:int}/sync-from-sql")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> SyncEducationFromSql(int id)
        {
            await _graphEducationService.MirrorEducationFromSqlAsync(id);
            return NoContent();
        }

        // PUT
        [HttpPut("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateEducation(int id, [FromBody] UpdateEducationGraphDto dto)
        {
            var success = await _graphEducationService.UpdateEducationNodeAsync(id, dto.Name, dto.Type, dto.Grade);
            return success ? NoContent() : NotFound();
        }

        // DELETE
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteEducation(int id)
        {
            var success = await _graphEducationService.DeleteEducationNodeAsync(id);
            return success ? NoContent() : NotFound();
        }
    }

    public class UpdateEducationGraphDto
    {
        public string? Name { get; set; }
        public string? Type { get; set; }
        public int? Grade { get; set; }
    }
}