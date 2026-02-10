using BaseFaq.Common.Infrastructure.ApiErrorHandling.Exception;
using BaseFaq.Common.Infrastructure.Core.Abstractions;
using BaseFaq.Faq.FaqWeb.Business.Faq.Abstractions;
using BaseFaq.Faq.FaqWeb.Business.Faq.Commands.CreateFaqContentRef;
using BaseFaq.Faq.FaqWeb.Business.Faq.Commands.UpdateFaqContentRef;
using BaseFaq.Faq.FaqWeb.Business.Faq.Queries.GetFaqContentRef;
using BaseFaq.Faq.FaqWeb.Business.Faq.Queries.GetFaqContentRefList;
using BaseFaq.Models.Common.Dtos;
using BaseFaq.Models.Common.Enums;
using BaseFaq.Models.Faq.Dtos.FaqContentRef;
using MediatR;
using System.Net;

namespace BaseFaq.Faq.FaqWeb.Business.Faq.Service;

public class FaqContentRefService(IMediator mediator, ISessionService sessionService) : IFaqContentRefService
{
    public async Task<FaqContentRefDto> Create(FaqContentRefCreateRequestDto requestDto, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        var tenantId = sessionService.GetTenantId(AppEnum.FaqWeb);

        var command = new FaqContentRefsCreateFaqContentRefCommand
        {
            FaqId = requestDto.FaqId,
            ContentRefId = requestDto.ContentRefId,
            TenantId = tenantId
        };

        var id = await mediator.Send(command, token);

        var result = await mediator.Send(new FaqContentRefsGetFaqContentRefQuery { Id = id }, token);
        if (result is null)
        {
            throw new ApiErrorException(
                $"Created FAQ content reference '{id}' was not found.",
                errorCode: (int)HttpStatusCode.InternalServerError);
        }

        return result;
    }

    public Task<PagedResultDto<FaqContentRefDto>> GetAll(
        FaqContentRefGetAllRequestDto requestDto,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        return mediator.Send(new FaqContentRefsGetFaqContentRefListQuery { Request = requestDto }, token);
    }

    public async Task<FaqContentRefDto> GetById(Guid id, CancellationToken token)
    {
        var result = await mediator.Send(new FaqContentRefsGetFaqContentRefQuery { Id = id }, token);
        if (result is null)
        {
            throw new ApiErrorException(
                $"FAQ content reference '{id}' was not found.",
                errorCode: (int)HttpStatusCode.NotFound);
        }

        return result;
    }

    public async Task<FaqContentRefDto> Update(Guid id, FaqContentRefUpdateRequestDto requestDto,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        var command = new FaqContentRefsUpdateFaqContentRefCommand
        {
            Id = id,
            FaqId = requestDto.FaqId,
            ContentRefId = requestDto.ContentRefId
        };

        await mediator.Send(command, token);

        return await GetById(id, token);
    }
}