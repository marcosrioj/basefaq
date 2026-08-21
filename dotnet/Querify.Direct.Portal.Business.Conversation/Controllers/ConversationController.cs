using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Querify.Direct.Portal.Business.Conversation.Abstractions;
using Querify.Models.Common.Dtos;
using Querify.Models.Direct.Dtos.Conversation;

namespace Querify.Direct.Portal.Business.Conversation.Controllers;

[Authorize]
[ApiController]
[Route("api/direct/conversations")]
public sealed class ConversationController(IConversationService service) : ControllerBase
{
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ConversationDetailDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken) =>
        Ok(await service.GetById(id, cancellationToken));

    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<ConversationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] ConversationGetAllRequestDto request,
        CancellationToken cancellationToken) =>
        Ok(await service.GetAll(request, cancellationToken));

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(
        [FromBody] ConversationCreateRequestDto request,
        CancellationToken cancellationToken) =>
        StatusCode(StatusCodes.Status201Created, await service.Create(request, cancellationToken));

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] ConversationUpdateRequestDto request,
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
