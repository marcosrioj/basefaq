using BaseFaq.AI.Common.Contracts.Generation;
using MediatR;

namespace BaseFaq.AI.Generation.Business.Worker.Commands.ProcessFaqGenerationRequested;

public sealed record ProcessFaqGenerationRequestedCommand(
    FaqGenerationRequestedV1 Message,
    string HandlerName,
    string MessageId) : IRequest;