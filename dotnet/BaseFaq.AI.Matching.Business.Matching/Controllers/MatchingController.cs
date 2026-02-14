using BaseFaq.AI.Matching.Business.Matching.Commands.RequestMatching;
using BaseFaq.AI.Matching.Business.Matching.Queries.GetMatchingStatus;
using BaseFaq.Models.Ai.Dtos.Matching;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BaseFaq.AI.Matching.Business.Matching.Controllers;

[ApiController]
[Route("api/ai/matching")]
public class MatchingController(IMediator mediator) : ControllerBase
{
    [HttpGet("status")]
    [ProducesResponseType(typeof(MatchingStatusResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStatus(CancellationToken token)
    {
        var result = await mediator.Send(new MatchingGetStatusQuery(), token);
        return Ok(result);
    }

    [HttpPost("requests")]
    [ProducesResponseType(typeof(MatchingRequestAcceptedResponse), StatusCodes.Status202Accepted)]
    public async Task<IActionResult> RequestMatching(
        [FromBody] MatchingRequestDto request,
        [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey,
        CancellationToken token)
    {
        var result = await mediator.Send(new MatchingRequestCommand(request, idempotencyKey), token);
        return Accepted(result);
    }
}