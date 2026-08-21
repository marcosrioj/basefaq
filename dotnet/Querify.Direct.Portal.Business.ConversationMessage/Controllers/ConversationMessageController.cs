using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Querify.Direct.Portal.Business.ConversationMessage.Abstractions;
using Querify.Models.Common.Dtos;
using Querify.Models.Direct.Dtos.ConversationMessage;

namespace Querify.Direct.Portal.Business.ConversationMessage.Controllers;

[Authorize]
[ApiController]
[Route("api/direct/conversation-messages")]
public sealed class ConversationMessageController(IConversationMessageService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<ConversationMessageDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] ConversationMessageGetAllRequestDto request,
        CancellationToken cancellationToken) =>
        Ok(await service.GetAll(request, cancellationToken));

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(
        [FromBody] ConversationMessageCreateRequestDto request,
        CancellationToken cancellationToken) =>
        StatusCode(StatusCodes.Status201Created, await service.Create(request, cancellationToken));
}
