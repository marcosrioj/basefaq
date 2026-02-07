using BaseFaq.Models.Common.Dtos;
using BaseFaq.Models.Tenant.Dtos.TenantConnection;
using BaseFaq.Tenant.TenantWeb.Business.Tenant.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BaseFaq.Tenant.TenantWeb.Business.Tenant.Controllers;

[Authorize]
[ApiController]
[Route("api/tenant/tenant-connections")]
public class TenantConnectionController(ITenantConnectionService tenantConnectionService) : ControllerBase
{
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TenantConnectionDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken token)
    {
        var result = await tenantConnectionService.GetById(id, token);
        return Ok(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<TenantConnectionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] TenantConnectionGetAllRequestDto requestDto,
        CancellationToken token)
    {
        var result = await tenantConnectionService.GetAll(requestDto, token);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TenantConnectionDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] TenantConnectionCreateRequestDto dto,
        CancellationToken token)
    {
        var result = await tenantConnectionService.Create(dto, token);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(TenantConnectionDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(Guid id, [FromBody] TenantConnectionUpdateRequestDto dto,
        CancellationToken token)
    {
        var result = await tenantConnectionService.Update(id, dto, token);
        return Ok(result);
    }
}