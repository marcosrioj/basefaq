using BaseFaq.AI.Matching.Business.Matching.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BaseFaq.AI.Matching.Business.Matching.Controllers;

[ApiController]
[Route("api/ai/matching")]
public class MatchingController(IMatchingStatusService matchingStatusService) : ControllerBase
{
    [HttpGet("status")]
    [ProducesResponseType(typeof(MatchingStatusResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStatus(CancellationToken token)
    {
        var result = await matchingStatusService.GetStatusAsync(token);
        return Ok(result);
    }

    [HttpPost("requests")]
    [ProducesResponseType(typeof(MatchingRequestAcceptedResponse), StatusCodes.Status202Accepted)]
    public async Task<IActionResult> RequestMatching(
        [FromBody] MatchingRequestDto request,
        [FromServices] IMatchingRequestService matchingRequestService,
        CancellationToken token)
    {
        var result = await matchingRequestService.EnqueueAsync(request, token);
        return Accepted(result);
    }
}