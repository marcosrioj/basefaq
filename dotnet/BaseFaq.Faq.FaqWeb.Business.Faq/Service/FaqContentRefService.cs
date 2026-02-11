using BaseFaq.Common.Infrastructure.ApiErrorHandling.Exception;
using BaseFaq.Faq.FaqWeb.Business.Faq.Abstractions;
using BaseFaq.Faq.FaqWeb.Business.Faq.Commands.CreateFaqContentRef;
using BaseFaq.Faq.FaqWeb.Business.Faq.Commands.DeleteFaqContentRef;
using BaseFaq.Faq.FaqWeb.Business.Faq.Commands.UpdateFaqContentRef;
using BaseFaq.Faq.FaqWeb.Business.Faq.Queries.GetFaqContentRef;
using BaseFaq.Faq.FaqWeb.Business.Faq.Queries.GetFaqContentRefList;
using BaseFaq.Models.Common.Dtos;
using BaseFaq.Models.Faq.Dtos.FaqContentRef;
using MediatR;
using System.Net;

namespace BaseFaq.Faq.FaqWeb.Business.Faq.Service;

public class FaqContentRefService(IMediator mediator) : IFaqContentRefService
{
    public async Task<FaqContentRefDto> Create(FaqContentRefCreateRequestDto requestDto, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        var command = new FaqContentRefsCreateFaqContentRefCommand
        {
            FaqId = requestDto.FaqId,
            ContentRefId = requestDto.ContentRefId
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

    public Task Delete(Guid id, CancellationToken token)
    {
        return mediator.Send(new FaqContentRefsDeleteFaqContentRefCommand { Id = id }, token);
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