using BaseFaq.Faq.Public.Business.Faq.Abstractions;
using BaseFaq.Models.Common.Dtos;
using BaseFaq.Models.Faq.Dtos.Faq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BaseFaq.Faq.Public.Business.Faq.Controllers;

[ApiController]
[Route("api/faqs/faq")]
public class FaqController(IFaqService faqService) : ControllerBase
{
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(FaqDetailDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(
        Guid id,
        [FromQuery] FaqGetRequestDto requestDto,
        CancellationToken token)
    {
        var result = await faqService.GetById(id, requestDto, token);
        return Ok(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<FaqDetailDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] FaqGetAllRequestDto requestDto, CancellationToken token)
    {
        var result = await faqService.GetAll(requestDto, token);
        return Ok(result);
    }
}