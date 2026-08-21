using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Querify.Broadcast.Portal.Business.Item.Abstractions;
using Querify.Models.Broadcast.Dtos.Item;
using Querify.Models.Common.Dtos;

namespace Querify.Broadcast.Portal.Business.Item.Controllers;

[Authorize]
[ApiController]
[Route("api/broadcast/items")]
public sealed class ItemController(IItemService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<ItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] ItemGetAllRequestDto request,
        CancellationToken cancellationToken) =>
        Ok(await service.GetAll(request, cancellationToken));

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(
        [FromBody] ItemCreateRequestDto request,
        CancellationToken cancellationToken) =>
        StatusCode(StatusCodes.Status201Created, await service.Create(request, cancellationToken));
}
