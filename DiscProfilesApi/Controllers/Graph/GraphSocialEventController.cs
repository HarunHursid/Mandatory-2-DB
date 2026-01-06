using System.Threading.Tasks;
using DiscProfilesApi.Services.GraphServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace DiscProfilesApi.Controllers.Graph
{
    [ApiController]
    [Route("api/graph/[controller]")]
    public class GraphSocialEventController : ControllerBase
    {
        private readonly GraphSocialEventService _graphSocialEventService;

        public GraphSocialEventController(GraphSocialEventService graphSocialEventService)
        {
            _graphSocialEventService = graphSocialEventService;
        }

        // GET
        [HttpGet("{id:int}/employees")]
        public async Task<IActionResult> GetEmployeesAtEvent(int id)
        {
            var data = await _graphSocialEventService.GetEmployeesAtEventAsync(id);
            return Ok(data);
        }

        // POST
        [HttpPost("{id:int}/sync-from-sql")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> SyncSocialEventFromSql(int id)
        {
            await _graphSocialEventService.MirrorSocialEventFromSqlAsync(id);
            return NoContent();
        }

        // PUT
        [HttpPut("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateSocialEvent(int id, [FromBody] UpdateSocialEventGraphDto dto)
        {
            var success = await _graphSocialEventService.UpdateSocialEventNodeAsync(id, dto.Name, dto.Description);
            return success ? NoContent() : NotFound();
        }

        // DELETE
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteSocialEvent(int id)
        {
            var success = await _graphSocialEventService.DeleteSocialEventNodeAsync(id);
            return success ? NoContent() : NotFound();
        }
    }

    public class UpdateSocialEventGraphDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}