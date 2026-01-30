using BaseFaq.Common.Infrastructure.Core.Abstractions;
using BaseFaq.Faq.Business.Faq.Abstractions;
using BaseFaq.Faq.Business.Faq.Commands.CreateFaq;
using BaseFaq.Faq.Business.Faq.Queries.GetFaq;
using BaseFaq.Models.Faq.Dtos.Faq;
using MediatR;

namespace BaseFaq.Faq.Business.Faq.Service;

public class FaqService(IMediator mediator, ISessionService sessionService) : IFaqService
{
    public async Task<FaqDto> Create(FaqCreateRequestDto requestDto, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        var tenantId = sessionService.TenantId
                       ?? throw new InvalidOperationException("TenantId is missing from the current session.");

        var command = new FaqsCreateFaqCommand
        {
            Name = requestDto.Name,
            Language = requestDto.Language,
            Status = requestDto.Status,
            SortType = requestDto.SortType,
            TenantId = tenantId
        };

        var id = await mediator.Send(command, token);

        var result = await mediator.Send(new FaqsGetFaqQuery { Id = id }, token);
        if (result is null)
        {
            throw new InvalidOperationException($"Created FAQ '{id}' was not found.");
        }

        return result;
    }

    public async Task<FaqDto> GetById(Guid id, CancellationToken token)
    {
        var result = await mediator.Send(new FaqsGetFaqQuery { Id = id }, token);
        if (result is null)
        {
            throw new KeyNotFoundException($"FAQ '{id}' was not found.");
        }

        return result;
    }
}