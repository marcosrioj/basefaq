using BaseFaq.Faq.Business.Faq.Abstractions;
using BaseFaq.Faq.Business.Faq.Authorization;
using BaseFaq.Models.Faq.Dtos.Faq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BaseFaq.Faq.Business.Faq.Controllers;

[Authorize]
[ApiController]
[Route("api/faq/faqs")]
public class FaqController(IFaqService faqService) : ControllerBase
{
    [Authorize(Policy = FaqAuthorizationPolicies.Create)]
    [HttpPost]
    [ProducesResponseType(typeof(FaqDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Create([FromBody] FaqCreateRequestDto dto, CancellationToken token)
    {
        var result = await faqService.Create(dto, token);

        return Ok(result);
    }
}