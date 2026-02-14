using BaseFaq.AI.Common.Contracts.Matching;
using BaseFaq.AI.Matching.Business.Worker.Commands.ProcessFaqMatchingRequested;
using MediatR;

namespace BaseFaq.AI.Matching.Business.Worker.Consumers;

public sealed class FaqMatchingRequestedConsumer(
    IMediator mediator) : MassTransit.IConsumer<FaqMatchingRequestedV1>
{
    public async Task Consume(MassTransit.ConsumeContext<FaqMatchingRequestedV1> context)
    {
        var handlerName = nameof(FaqMatchingRequestedConsumer);
        var messageId = context.MessageId?.ToString("N") ?? context.Message.CorrelationId.ToString("N");
        var message = context.Message;

        await mediator.Send(
            new ProcessFaqMatchingRequestedCommand(message, handlerName, messageId),
            context.CancellationToken);
    }
}