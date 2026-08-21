using MediatR;
using Querify.Direct.Portal.Business.ConversationMessage.Abstractions;
using Querify.Direct.Portal.Business.ConversationMessage.Commands.CreateConversationMessage;
using Querify.Direct.Portal.Business.ConversationMessage.Queries.GetConversationMessageList;
using Querify.Models.Common.Dtos;
using Querify.Models.Direct.Dtos.ConversationMessage;

namespace Querify.Direct.Portal.Business.ConversationMessage.Service;

public sealed class ConversationMessageService(IMediator mediator) : IConversationMessageService
{
    public Task<Guid> Create(ConversationMessageCreateRequestDto request, CancellationToken cancellationToken) =>
        mediator.Send(new ConversationMessagesCreateCommand { Request = request }, cancellationToken);

    public Task<PagedResultDto<ConversationMessageDto>> GetAll(
        ConversationMessageGetAllRequestDto request,
        CancellationToken cancellationToken) =>
        mediator.Send(new ConversationMessagesGetListQuery { Request = request }, cancellationToken);
}
