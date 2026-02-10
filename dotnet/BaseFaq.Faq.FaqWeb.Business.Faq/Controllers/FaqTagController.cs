using BaseFaq.Faq.FaqWeb.Business.Faq.Abstractions;
using BaseFaq.Models.Common.Dtos;
using BaseFaq.Models.Faq.Dtos.FaqTag;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BaseFaq.Faq.FaqWeb.Business.Faq.Controllers;

[Authorize]
[ApiController]
[Route("api/faqs/faq-tag")]
public class FaqTagController(IFaqTagService faqTagService) : ControllerBase
{
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(FaqTagDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken token)
    {
        var result = await faqTagService.GetById(id, token);
        return Ok(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<FaqTagDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] FaqTagGetAllRequestDto requestDto, CancellationToken token)
    {
        var result = await faqTagService.GetAll(requestDto, token);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(FaqTagDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] FaqTagCreateRequestDto dto, CancellationToken token)
    {
        var result = await faqTagService.Create(dto, token);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(FaqTagDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(Guid id, [FromBody] FaqTagUpdateRequestDto dto, CancellationToken token)
    {
        var result = await faqTagService.Update(id, dto, token);
        return Ok(result);
    }
}