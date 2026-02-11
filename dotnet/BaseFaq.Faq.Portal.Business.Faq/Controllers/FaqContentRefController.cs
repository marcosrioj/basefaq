using BaseFaq.Faq.Portal.Business.Faq.Abstractions;
using BaseFaq.Models.Common.Dtos;
using BaseFaq.Models.Faq.Dtos.FaqContentRef;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BaseFaq.Faq.Portal.Business.Faq.Controllers;

[Authorize]
[ApiController]
[Route("api/faqs/faq-content-ref")]
public class FaqContentRefController(IFaqContentRefService faqContentRefService) : ControllerBase
{
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(FaqContentRefDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken token)
    {
        var result = await faqContentRefService.GetById(id, token);
        return Ok(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<FaqContentRefDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] FaqContentRefGetAllRequestDto requestDto,
        CancellationToken token)
    {
        var result = await faqContentRefService.GetAll(requestDto, token);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(FaqContentRefDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] FaqContentRefCreateRequestDto dto, CancellationToken token)
    {
        var result = await faqContentRefService.Create(dto, token);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(FaqContentRefDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] FaqContentRefUpdateRequestDto dto,
        CancellationToken token)
    {
        var result = await faqContentRefService.Update(id, dto, token);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken token)
    {
        await faqContentRefService.Delete(id, token);
        return NoContent();
    }
}