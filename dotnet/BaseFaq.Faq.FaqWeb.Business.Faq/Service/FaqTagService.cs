using BaseFaq.Common.Infrastructure.ApiErrorHandling.Exception;
using BaseFaq.Faq.FaqWeb.Business.Faq.Abstractions;
using BaseFaq.Faq.FaqWeb.Business.Faq.Commands.CreateFaqTag;
using BaseFaq.Faq.FaqWeb.Business.Faq.Commands.DeleteFaqTag;
using BaseFaq.Faq.FaqWeb.Business.Faq.Commands.UpdateFaqTag;
using BaseFaq.Faq.FaqWeb.Business.Faq.Queries.GetFaqTag;
using BaseFaq.Faq.FaqWeb.Business.Faq.Queries.GetFaqTagList;
using BaseFaq.Models.Common.Dtos;
using BaseFaq.Models.Faq.Dtos.FaqTag;
using MediatR;
using System.Net;

namespace BaseFaq.Faq.FaqWeb.Business.Faq.Service;

public class FaqTagService(IMediator mediator) : IFaqTagService
{
    public async Task<FaqTagDto> Create(FaqTagCreateRequestDto requestDto, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        var command = new FaqTagsCreateFaqTagCommand
        {
            FaqId = requestDto.FaqId,
            TagId = requestDto.TagId
        };

        var id = await mediator.Send(command, token);

        var result = await mediator.Send(new FaqTagsGetFaqTagQuery { Id = id }, token);
        if (result is null)
        {
            throw new ApiErrorException(
                $"Created FAQ tag '{id}' was not found.",
                errorCode: (int)HttpStatusCode.InternalServerError);
        }

        return result;
    }

    public Task<PagedResultDto<FaqTagDto>> GetAll(FaqTagGetAllRequestDto requestDto, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        return mediator.Send(new FaqTagsGetFaqTagListQuery { Request = requestDto }, token);
    }

    public Task Delete(Guid id, CancellationToken token)
    {
        return mediator.Send(new FaqTagsDeleteFaqTagCommand { Id = id }, token);
    }

    public async Task<FaqTagDto> GetById(Guid id, CancellationToken token)
    {
        var result = await mediator.Send(new FaqTagsGetFaqTagQuery { Id = id }, token);
        if (result is null)
        {
            throw new ApiErrorException(
                $"FAQ tag '{id}' was not found.",
                errorCode: (int)HttpStatusCode.NotFound);
        }

        return result;
    }

    public async Task<FaqTagDto> Update(Guid id, FaqTagUpdateRequestDto requestDto, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        var command = new FaqTagsUpdateFaqTagCommand
        {
            Id = id,
            FaqId = requestDto.FaqId,
            TagId = requestDto.TagId
        };

        await mediator.Send(command, token);

        return await GetById(id, token);
    }
}