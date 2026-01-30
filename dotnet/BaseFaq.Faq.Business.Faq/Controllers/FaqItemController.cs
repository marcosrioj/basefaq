using BaseFaq.Faq.Business.Faq.Abstractions;
using BaseFaq.Faq.Business.Faq.Authorization;
using BaseFaq.Models.Common.Dtos;
using BaseFaq.Models.Faq.Dtos.FaqItem;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BaseFaq.Faq.Business.Faq.Controllers;

[Authorize]
[ApiController]
[Route("api/faqs/faq-item")]
public class FaqItemController(IFaqItemService faqItemService) : ControllerBase
{
    [Authorize(Policy = FaqItemAuthorizationPolicies.Get)]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(FaqItemDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken token)
    {
        var result = await faqItemService.GetById(id, token);
        return Ok(result);
    }

    [Authorize(Policy = FaqItemAuthorizationPolicies.GetList)]
    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<FaqItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] FaqItemGetAllRequestDto requestDto, CancellationToken token)
    {
        var result = await faqItemService.GetAll(requestDto, token);
        return Ok(result);
    }

    [Authorize(Policy = FaqItemAuthorizationPolicies.Create)]
    [HttpPost]
    [ProducesResponseType(typeof(FaqItemDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] FaqItemCreateRequestDto dto, CancellationToken token)
    {
        var result = await faqItemService.Create(dto, token);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [Authorize(Policy = FaqItemAuthorizationPolicies.Update)]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(FaqItemDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(Guid id, [FromBody] FaqItemUpdateRequestDto dto, CancellationToken token)
    {
        var result = await faqItemService.Update(id, dto, token);
        return Ok(result);
    }
}