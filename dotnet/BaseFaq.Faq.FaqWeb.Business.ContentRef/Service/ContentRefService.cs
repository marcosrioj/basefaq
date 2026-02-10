using BaseFaq.Common.Infrastructure.ApiErrorHandling.Exception;
using BaseFaq.Common.Infrastructure.Core.Abstractions;
using BaseFaq.Faq.FaqWeb.Business.ContentRef.Abstractions;
using BaseFaq.Faq.FaqWeb.Business.ContentRef.Commands.CreateContentRef;
using BaseFaq.Faq.FaqWeb.Business.ContentRef.Commands.UpdateContentRef;
using BaseFaq.Faq.FaqWeb.Business.ContentRef.Queries.GetContentRef;
using BaseFaq.Faq.FaqWeb.Business.ContentRef.Queries.GetContentRefList;
using BaseFaq.Models.Common.Dtos;
using BaseFaq.Models.Common.Enums;
using BaseFaq.Models.Faq.Dtos.ContentRef;
using MediatR;
using System.Net;

namespace BaseFaq.Faq.FaqWeb.Business.ContentRef.Service;

public class ContentRefService(IMediator mediator, ISessionService sessionService) : IContentRefService
{
    public async Task<ContentRefDto> Create(ContentRefCreateRequestDto requestDto, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        var tenantId = sessionService.GetTenantId(AppEnum.FaqWeb);

        var command = new ContentRefsCreateContentRefCommand
        {
            Kind = requestDto.Kind,
            Locator = requestDto.Locator,
            Label = requestDto.Label,
            Scope = requestDto.Scope,
            TenantId = tenantId
        };

        var id = await mediator.Send(command, token);

        var result = await mediator.Send(new ContentRefsGetContentRefQuery { Id = id }, token);
        if (result is null)
        {
            throw new ApiErrorException(
                $"Created content reference '{id}' was not found.",
                errorCode: (int)HttpStatusCode.InternalServerError);
        }

        return result;
    }

    public Task<PagedResultDto<ContentRefDto>> GetAll(ContentRefGetAllRequestDto requestDto, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        return mediator.Send(new ContentRefsGetContentRefListQuery { Request = requestDto }, token);
    }

    public async Task<ContentRefDto> GetById(Guid id, CancellationToken token)
    {
        var result = await mediator.Send(new ContentRefsGetContentRefQuery { Id = id }, token);
        if (result is null)
        {
            throw new ApiErrorException(
                $"Content reference '{id}' was not found.",
                errorCode: (int)HttpStatusCode.NotFound);
        }

        return result;
    }

    public async Task<ContentRefDto> Update(Guid id, ContentRefUpdateRequestDto requestDto, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        var command = new ContentRefsUpdateContentRefCommand
        {
            Id = id,
            Kind = requestDto.Kind,
            Locator = requestDto.Locator,
            Label = requestDto.Label,
            Scope = requestDto.Scope
        };

        await mediator.Send(command, token);

        return await GetById(id, token);
    }
}