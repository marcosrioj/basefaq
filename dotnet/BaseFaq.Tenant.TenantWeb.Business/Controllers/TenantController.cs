using BaseFaq.Models.Common.Dtos;
using BaseFaq.Models.Tenant.Dtos.Tenant;
using BaseFaq.Tenant.TenantWeb.Business.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BaseFaq.Tenant.TenantWeb.Business.Controllers;

[Authorize]
[ApiController]
[Route("api/tenant/tenants")]
public class TenantController(ITenantService tenantService) : ControllerBase
{
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TenantDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken token)
    {
        var result = await tenantService.GetById(id, token);
        return Ok(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<TenantDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] TenantGetAllRequestDto requestDto, CancellationToken token)
    {
        var result = await tenantService.GetAll(requestDto, token);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TenantDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] TenantCreateRequestDto dto, CancellationToken token)
    {
        var result = await tenantService.Create(dto, token);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(TenantDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(Guid id, [FromBody] TenantUpdateRequestDto dto, CancellationToken token)
    {
        var result = await tenantService.Update(id, dto, token);
        return Ok(result);
    }

    [HttpPost("default")]
    [ProducesResponseType(typeof(TenantDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> SetDefault([FromQuery] Guid? tenantId, CancellationToken token)
    {
        var result = await tenantService.SetDefault(tenantId, token);
        return Ok(result);
    }
}