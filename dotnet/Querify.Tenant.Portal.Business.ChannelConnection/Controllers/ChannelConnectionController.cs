using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Querify.Common.Infrastructure.Core.Extensions;
using Querify.Models.Common.Dtos;
using Querify.Models.Tenant.Dtos.ChannelConnection;
using Querify.Tenant.Portal.Business.ChannelConnection.Abstractions;

namespace Querify.Tenant.Portal.Business.ChannelConnection.Controllers;

[Authorize]
[ApiController]
[Route("api/tenant/channel-connections")]
public sealed class ChannelConnectionController(IChannelConnectionService service) : ControllerBase
{
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ChannelConnectionDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken) =>
        Ok(await service.GetById(HttpContext.GetTenantIdFromHeader(), id, cancellationToken));

    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<ChannelConnectionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] ChannelConnectionGetAllRequestDto request,
        CancellationToken cancellationToken) =>
        Ok(await service.GetAll(HttpContext.GetTenantIdFromHeader(), request, cancellationToken));

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(
        [FromBody] ChannelConnectionCreateRequestDto request,
        CancellationToken cancellationToken) =>
        StatusCode(
            StatusCodes.Status201Created,
            await service.Create(HttpContext.GetTenantIdFromHeader(), request, cancellationToken));

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] ChannelConnectionUpdateRequestDto request,
        CancellationToken cancellationToken) =>
        Ok(await service.Update(HttpContext.GetTenantIdFromHeader(), id, request, cancellationToken));

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await service.Delete(HttpContext.GetTenantIdFromHeader(), id, cancellationToken);
        return NoContent();
    }
}
