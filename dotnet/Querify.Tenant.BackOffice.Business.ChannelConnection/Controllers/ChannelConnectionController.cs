using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Querify.Models.Common.Dtos;
using Querify.Models.Tenant.Dtos.ChannelConnection;
using Querify.Tenant.BackOffice.Business.ChannelConnection.Abstractions;

namespace Querify.Tenant.BackOffice.Business.ChannelConnection.Controllers;

[Authorize]
[ApiController]
[Route("api/tenant/channel-connections")]
public sealed class ChannelConnectionController(IChannelConnectionBackOfficeService service) : ControllerBase
{
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ChannelConnectionDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken) =>
        Ok(await service.GetById(id, cancellationToken));

    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<ChannelConnectionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] ChannelConnectionGetAllRequestDto request,
        CancellationToken cancellationToken) =>
        Ok(await service.GetAll(request, cancellationToken));

    [HttpPut("{id:guid}/operational-state")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateOperationalState(
        Guid id,
        [FromBody] ChannelConnectionOperationalUpdateRequestDto request,
        CancellationToken cancellationToken) =>
        Ok(await service.UpdateOperationalState(id, request, cancellationToken));
}
