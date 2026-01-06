using System.Threading.Tasks;
using DiscProfilesApi.Services.GraphServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace DiscProfilesApi.Controllers.Graph
{
    [ApiController]
    [Route("api/graph/[controller]")]
    public class GraphProjectsDiscProfilesController : ControllerBase
    {
        private readonly GraphProjectsDiscProfilesService _graphProjectsDiscProfilesService;

        public GraphProjectsDiscProfilesController(GraphProjectsDiscProfilesService graphProjectsDiscProfilesService)
        {
            _graphProjectsDiscProfilesService = graphProjectsDiscProfilesService;
        }

        // GET
        [HttpGet("project/{id:int}")]
        public async Task<IActionResult> GetDiscProfilesForProject(int id)
        {
            var data = await _graphProjectsDiscProfilesService.GetDiscProfilesForProjectAsync(id);
            return Ok(data);
        }

        // POST
        [HttpPost("{id:int}/sync-from-sql")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> SyncProjectsDiscProfileFromSql(int id)
        {
            await _graphProjectsDiscProfilesService.MirrorProjectsDiscProfileFromSqlAsync(id);
            return NoContent();
        }

        // PUT
        [HttpPut("project/{projectId:int}/profile/{oldProfileId:int}/to/{newProfileId:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateProjectDiscProfileRelation(int projectId, int oldProfileId, int newProfileId)
        {
            var success = await _graphProjectsDiscProfilesService.UpdateProjectDiscProfileRelationAsync(projectId, oldProfileId, newProfileId);
            return success ? NoContent() : NotFound();
        }

        // DELETE
        [HttpDelete("project/{projectId:int}/profile/{profileId:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteProjectDiscProfileRelation(int projectId, int profileId)
        {
            var success = await _graphProjectsDiscProfilesService.DeleteProjectDiscProfileRelationAsync(projectId, profileId);
            return success ? NoContent() : NotFound();
        }
    }
}