using BaseFaq.AI.Generation.Business.Generation.Abstractions;
using BaseFaq.Models.Ai.Dtos.Generation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BaseFaq.AI.Generation.Business.Generation.Controllers;

[ApiController]
[Route("api/ai/generation")]
public class GenerationController(
    IGenerationStatusService generationStatusService,
    IGenerationRequestService generationRequestService) : ControllerBase
{
    [HttpGet("status")]
    [ProducesResponseType(typeof(GenerationStatusResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStatus(CancellationToken token)
    {
        var result = await generationStatusService.GetStatusAsync(token);
        return Ok(result);
    }

    [HttpPost("requests")]
    [ProducesResponseType(typeof(GenerationRequestAcceptedResponse), StatusCodes.Status202Accepted)]
    public async Task<IActionResult> RequestGeneration(
        [FromBody] GenerationRequestDto request,
        [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey,
        CancellationToken token)
    {
        var result = await generationRequestService.EnqueueAsync(request, idempotencyKey, token);
        return Accepted(result);
    }
}