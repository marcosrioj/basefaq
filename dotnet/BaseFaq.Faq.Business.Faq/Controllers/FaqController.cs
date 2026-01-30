using BaseFaq.Faq.Business.Faq.Abstractions;
using BaseFaq.Faq.Business.Faq.Authorization;
using BaseFaq.Models.Faq.Dtos.Faq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BaseFaq.Faq.Business.Faq.Controllers;

[Authorize]
[ApiController]
[Route("api/faqs/faq")]
public class FaqController(IFaqService faqService) : ControllerBase
{
    [Authorize(Policy = FaqAuthorizationPolicies.Get)]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(FaqDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken token)
    {
        var result = await faqService.GetById(id, token);
        return Ok(result);
    }

    [Authorize(Policy = FaqAuthorizationPolicies.Create)]
    [HttpPost]
    [ProducesResponseType(typeof(FaqDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] FaqCreateRequestDto dto, CancellationToken token)
    {
        var result = await faqService.Create(dto, token);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
}