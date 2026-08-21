using Querify.Models.Common.Dtos;

namespace Querify.Models.Direct.Dtos.Contact;

public sealed class ContactGetAllRequestDto : PagedAndSortedResultRequestDto
{
    public string? SearchText { get; set; }
}
