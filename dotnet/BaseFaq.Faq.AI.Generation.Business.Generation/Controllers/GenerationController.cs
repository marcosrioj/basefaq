using BaseFaq.Faq.AI.Generation.Business.Generation.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BaseFaq.Faq.AI.Generation.Business.Generation.Controllers;

[ApiController]
[Route("api/ai/generation")]
public class GenerationController(IGenerationStatusService generationStatusService) : ControllerBase
{
    [HttpGet("status")]
    [ProducesResponseType(typeof(GenerationStatusResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStatus(CancellationToken token)
    {
        var result = await generationStatusService.GetStatusAsync(token);
        return Ok(result);
    }
}