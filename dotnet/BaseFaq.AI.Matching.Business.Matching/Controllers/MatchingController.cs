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
}