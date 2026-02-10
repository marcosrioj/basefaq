using BaseFaq.Faq.FaqWeb.Business.FaqItem.Abstractions;
using BaseFaq.Models.Common.Dtos;
using BaseFaq.Models.Faq.Dtos.FaqItem;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BaseFaq.Faq.FaqWeb.Business.FaqItem.Controllers;

[Authorize]
[ApiController]
[Route("api/faqs/faq-item")]
public class FaqItemController(IFaqItemService faqItemService) : ControllerBase
{
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(FaqItemDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken token)
    {
        var result = await faqItemService.GetById(id, token);
        return Ok(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<FaqItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] FaqItemGetAllRequestDto requestDto, CancellationToken token)
    {
        var result = await faqItemService.GetAll(requestDto, token);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(FaqItemDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] FaqItemCreateRequestDto dto, CancellationToken token)
    {
        var result = await faqItemService.Create(dto, token);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(FaqItemDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(Guid id, [FromBody] FaqItemUpdateRequestDto dto, CancellationToken token)
    {
        var result = await faqItemService.Update(id, dto, token);
        return Ok(result);
    }
}