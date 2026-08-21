using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Querify.Broadcast.Portal.Business.Thread.Abstractions;
using Querify.Models.Broadcast.Dtos.Thread;
using Querify.Models.Common.Dtos;

namespace Querify.Broadcast.Portal.Business.Thread.Controllers;

[Authorize]
[ApiController]
[Route("api/broadcast/threads")]
public sealed class ThreadController(IThreadService service) : ControllerBase
{
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ThreadDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken) =>
        Ok(await service.GetById(id, cancellationToken));

    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<ThreadDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] ThreadGetAllRequestDto request,
        CancellationToken cancellationToken) =>
        Ok(await service.GetAll(request, cancellationToken));

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(
        [FromBody] ThreadCreateRequestDto request,
        CancellationToken cancellationToken) =>
        StatusCode(StatusCodes.Status201Created, await service.Create(request, cancellationToken));

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] ThreadUpdateRequestDto request,
        CancellationToken cancellationToken) =>
        Ok(await service.Update(id, request, cancellationToken));

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await service.Delete(id, cancellationToken);
        return NoContent();
    }
}
