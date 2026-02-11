using BaseFaq.Common.Infrastructure.ApiErrorHandling.Exception;
using BaseFaq.Faq.FaqWeb.Business.Tag.Abstractions;
using BaseFaq.Faq.FaqWeb.Business.Tag.Commands.DeleteTag;
using BaseFaq.Faq.FaqWeb.Business.Tag.Commands.CreateTag;
using BaseFaq.Faq.FaqWeb.Business.Tag.Commands.UpdateTag;
using BaseFaq.Faq.FaqWeb.Business.Tag.Queries.GetTag;
using BaseFaq.Faq.FaqWeb.Business.Tag.Queries.GetTagList;
using BaseFaq.Models.Common.Dtos;
using BaseFaq.Models.Faq.Dtos.Tag;
using MediatR;
using System.Net;

namespace BaseFaq.Faq.FaqWeb.Business.Tag.Service;

public class TagService(IMediator mediator) : ITagService
{
    public async Task<TagDto> Create(TagCreateRequestDto requestDto, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        var command = new TagsCreateTagCommand
        {
            Value = requestDto.Value
        };

        var id = await mediator.Send(command, token);

        var result = await mediator.Send(new TagsGetTagQuery { Id = id }, token);
        if (result is null)
        {
            throw new ApiErrorException(
                $"Created tag '{id}' was not found.",
                errorCode: (int)HttpStatusCode.InternalServerError);
        }

        return result;
    }

    public Task<PagedResultDto<TagDto>> GetAll(TagGetAllRequestDto requestDto, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        return mediator.Send(new TagsGetTagListQuery { Request = requestDto }, token);
    }

    public Task Delete(Guid id, CancellationToken token)
    {
        return mediator.Send(new TagsDeleteTagCommand { Id = id }, token);
    }

    public async Task<TagDto> GetById(Guid id, CancellationToken token)
    {
        var result = await mediator.Send(new TagsGetTagQuery { Id = id }, token);
        if (result is null)
        {
            throw new ApiErrorException(
                $"Tag '{id}' was not found.",
                errorCode: (int)HttpStatusCode.NotFound);
        }

        return result;
    }

    public async Task<TagDto> Update(Guid id, TagUpdateRequestDto requestDto, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        var command = new TagsUpdateTagCommand
        {
            Id = id,
            Value = requestDto.Value
        };

        await mediator.Send(command, token);

        return await GetById(id, token);
    }
}