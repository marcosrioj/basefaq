using BaseFaq.Faq.Public.Business.FaqItem.Abstractions;
using BaseFaq.Models.Common.Dtos;
using BaseFaq.Models.Faq.Dtos.FaqItem;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BaseFaq.Faq.Public.Business.FaqItem.Controllers;

[ApiController]
[Route("api/faqs/faq-item")]
public class FaqItemController(IFaqItemService faqItemService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(FaqItemDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] FaqItemCreateRequestDto dto, CancellationToken token)
    {
        var result = await faqItemService.Create(dto, token);

        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(PagedResultDto<FaqItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search([FromQuery] FaqItemSearchRequestDto requestDto, CancellationToken token)
    {
        var result = await faqItemService.Search(requestDto, token);
        return Ok(result);
    }
}