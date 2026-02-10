using BaseFaq.Faq.FaqWeb.Business.Vote.Abstractions;
using BaseFaq.Models.Common.Dtos;
using BaseFaq.Models.Faq.Dtos.Vote;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BaseFaq.Faq.FaqWeb.Business.Vote.Controllers;

[Authorize]
[ApiController]
[Route("api/faqs/vote")]
public class VoteController(IVoteService voteService) : ControllerBase
{
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(VoteDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken token)
    {
        var result = await voteService.GetById(id, token);
        return Ok(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<VoteDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] VoteGetAllRequestDto requestDto, CancellationToken token)
    {
        var result = await voteService.GetAll(requestDto, token);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost]
    [ProducesResponseType(typeof(VoteDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] VoteCreateRequestDto dto, CancellationToken token)
    {
        var result = await voteService.Create(dto, token);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [AllowAnonymous]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(VoteDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(Guid id, [FromBody] VoteUpdateRequestDto dto, CancellationToken token)
    {
        var result = await voteService.Update(id, dto, token);
        return Ok(result);
    }
}