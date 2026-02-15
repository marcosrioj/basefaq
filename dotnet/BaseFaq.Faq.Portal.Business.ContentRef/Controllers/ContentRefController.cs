using BaseFaq.Faq.Portal.Business.ContentRef.Abstractions;
using BaseFaq.Models.Common.Dtos;
using BaseFaq.Models.Faq.Dtos.ContentRef;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BaseFaq.Faq.Portal.Business.ContentRef.Controllers;

[Authorize]
[ApiController]
[Route("api/faqs/content-ref")]
public class ContentRefController(IContentRefService contentRefService) : ControllerBase
{
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ContentRefDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken token)
    {
        var result = await contentRefService.GetById(id, token);
        return Ok(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<ContentRefDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] ContentRefGetAllRequestDto requestDto, CancellationToken token)
    {
        var result = await contentRefService.GetAll(requestDto, token);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] ContentRefCreateRequestDto dto, CancellationToken token)
    {
        var result = await contentRefService.Create(dto, token);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(Guid id, [FromBody] ContentRefUpdateRequestDto dto, CancellationToken token)
    {
        var result = await contentRefService.Update(id, dto, token);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken token)
    {
        await contentRefService.Delete(id, token);
        return NoContent();
    }
}