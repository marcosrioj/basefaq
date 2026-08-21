using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Querify.Direct.Portal.Business.Contact.Abstractions;
using Querify.Models.Common.Dtos;
using Querify.Models.Direct.Dtos.Contact;

namespace Querify.Direct.Portal.Business.Contact.Controllers;

[Authorize]
[ApiController]
[Route("api/direct/contacts")]
public sealed class ContactController(IContactService contactService) : ControllerBase
{
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ContactDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken) =>
        Ok(await contactService.GetById(id, cancellationToken));

    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<ContactDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] ContactGetAllRequestDto request,
        CancellationToken cancellationToken) =>
        Ok(await contactService.GetAll(request, cancellationToken));

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(
        [FromBody] ContactCreateRequestDto request,
        CancellationToken cancellationToken) =>
        StatusCode(StatusCodes.Status201Created, await contactService.Create(request, cancellationToken));

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] ContactUpdateRequestDto request,
        CancellationToken cancellationToken) =>
        Ok(await contactService.Update(id, request, cancellationToken));

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await contactService.Delete(id, cancellationToken);
        return NoContent();
    }
}
