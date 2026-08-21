using MediatR;
using Querify.Direct.Portal.Business.Conversation.Abstractions;
using Querify.Direct.Portal.Business.Conversation.Commands.CreateConversation;
using Querify.Direct.Portal.Business.Conversation.Commands.DeleteConversation;
using Querify.Direct.Portal.Business.Conversation.Commands.UpdateConversation;
using Querify.Direct.Portal.Business.Conversation.Queries.GetConversation;
using Querify.Direct.Portal.Business.Conversation.Queries.GetConversationList;
using Querify.Models.Common.Dtos;
using Querify.Models.Direct.Dtos.Conversation;

namespace Querify.Direct.Portal.Business.Conversation.Service;

public sealed class ConversationService(IMediator mediator) : IConversationService
{
    public Task<Guid> Create(ConversationCreateRequestDto request, CancellationToken cancellationToken) =>
        mediator.Send(new ConversationsCreateCommand { Request = request }, cancellationToken);

    public Task<Guid> Update(Guid id, ConversationUpdateRequestDto request, CancellationToken cancellationToken) =>
        mediator.Send(new ConversationsUpdateCommand { Id = id, Request = request }, cancellationToken);

    public Task Delete(Guid id, CancellationToken cancellationToken) =>
        mediator.Send(new ConversationsDeleteCommand { Id = id }, cancellationToken);

    public Task<ConversationDetailDto> GetById(Guid id, CancellationToken cancellationToken) =>
        mediator.Send(new ConversationsGetQuery { Id = id }, cancellationToken);

    public Task<PagedResultDto<ConversationDto>> GetAll(
        ConversationGetAllRequestDto request,
        CancellationToken cancellationToken) =>
        mediator.Send(new ConversationsGetListQuery { Request = request }, cancellationToken);
}
