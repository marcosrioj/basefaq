using BaseFaq.Common.Infrastructure.Core.Abstractions;
using BaseFaq.Faq.FaqWeb.Business.Faq.Abstractions;
using BaseFaq.Faq.FaqWeb.Business.Faq.Commands.CreateFaq;
using BaseFaq.Faq.FaqWeb.Business.Faq.Commands.UpdateFaq;
using BaseFaq.Faq.FaqWeb.Business.Faq.Queries.GetFaq;
using BaseFaq.Faq.FaqWeb.Business.Faq.Queries.GetFaqList;
using BaseFaq.Models.Common.Enums;
using BaseFaq.Models.Common.Dtos;
using BaseFaq.Models.Faq.Dtos.Faq;
using MediatR;

namespace BaseFaq.Faq.FaqWeb.Business.Faq.Service;

public class FaqService(IMediator mediator, ISessionService sessionService) : IFaqService
{
    public async Task<FaqDto> Create(FaqCreateRequestDto requestDto, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        var tenantId = sessionService.GetTenantId(AppEnum.FaqWeb);

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

    public Task<PagedResultDto<FaqDto>> GetAll(FaqGetAllRequestDto requestDto, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        return mediator.Send(new FaqsGetFaqListQuery { Request = requestDto }, token);
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

    public async Task<FaqDto> Update(Guid id, FaqUpdateRequestDto requestDto, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        var command = new FaqsUpdateFaqCommand
        {
            Id = id,
            Name = requestDto.Name,
            Language = requestDto.Language,
            Status = requestDto.Status,
            SortType = requestDto.SortType
        };

        await mediator.Send(command, token);

        return await GetById(id, token);
    }
}