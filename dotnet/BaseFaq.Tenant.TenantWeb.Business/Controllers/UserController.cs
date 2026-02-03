using BaseFaq.Models.Common.Dtos;
using BaseFaq.Models.Tenant.Dtos.User;
using BaseFaq.Tenant.TenantWeb.Business.Abstractions;
using BaseFaq.Tenant.TenantWeb.Business.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BaseFaq.Tenant.TenantWeb.Business.Controllers;

[Authorize]
[ApiController]
[Route("api/tenant/users")]
public class UserController(IUserService userService) : ControllerBase
{
    [Authorize(Policy = UserAuthorizationPolicies.Get)]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken token)
    {
        var result = await userService.GetById(id, token);
        return Ok(result);
    }

    [Authorize(Policy = UserAuthorizationPolicies.GetList)]
    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<UserDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] UserGetAllRequestDto requestDto, CancellationToken token)
    {
        var result = await userService.GetAll(requestDto, token);
        return Ok(result);
    }

    [Authorize(Policy = UserAuthorizationPolicies.Create)]
    [HttpPost]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] UserCreateRequestDto dto, CancellationToken token)
    {
        var result = await userService.Create(dto, token);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [Authorize(Policy = UserAuthorizationPolicies.Update)]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UserUpdateRequestDto dto, CancellationToken token)
    {
        var result = await userService.Update(id, dto, token);
        return Ok(result);
    }
}