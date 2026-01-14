using Microsoft.AspNetCore.Mvc;
using MinCloud.Internal.SDK;
using MinCloud.Internal.SDK.HttpClients;

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
        public async Task<IActionResult> GetAll(int offset = 0, int limit = 20, string? search = null, CancellationToken cancellationToken = default)
        {
            var spaces = await minCloudClient.Space.GetListAsync(offset, limit, search, cancellationToken);
            return Ok(spaces);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSpaceRequest request, CancellationToken cancellationToken)
        {
            var space = await minCloudClient.Space.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = space!.Id }, space);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSpaceRequest request, CancellationToken cancellationToken)
        {
            var space = await minCloudClient.Space.UpdateAsync(id, request, cancellationToken);
            return space is null ? NotFound() : Ok(space);
        }
    }
}
