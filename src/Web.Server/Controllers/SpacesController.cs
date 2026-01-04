using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinCloud.Internal.SDK;

namespace Web.Server.Controllers
{
    public class SpacesController(IMinCloudClient minCloudClient) : ApiControllerBase
    {
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var space = await minCloudClient.Space.GetByIdAsync(id, cancellationToken);
            return space is null ? NotFound() : Ok(space);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var spaces = await minCloudClient.Space.GetAllAsync(cancellationToken);
            return Ok(spaces ?? []);
        }

    }
}
