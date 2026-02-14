using BaseFaq.AI.Generation.Business.Generation.Commands.RequestGeneration;
using BaseFaq.AI.Generation.Business.Generation.Queries.GetGenerationStatus;
using BaseFaq.Models.Ai.Dtos.Generation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BaseFaq.AI.Generation.Business.Generation.Controllers;

[ApiController]
[Route("api/ai/generation")]
public class GenerationController(
    IMediator mediator) : ControllerBase
{
    [HttpGet("status")]
    [ProducesResponseType(typeof(GenerationStatusResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStatus(CancellationToken token)
    {
        var result = await mediator.Send(new GenerationGetStatusQuery(), token);
        return Ok(result);
    }

    [HttpPost("requests")]
    [ProducesResponseType(typeof(GenerationRequestAcceptedResponse), StatusCodes.Status202Accepted)]
    public async Task<IActionResult> RequestGeneration(
        [FromBody] GenerationRequestDto request,
        [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey,
        CancellationToken token)
    {
        var result = await mediator.Send(new GenerationRequestCommand(request, idempotencyKey), token);
        return Accepted(result);
    }
}