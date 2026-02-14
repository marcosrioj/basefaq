using BaseFaq.Models.Ai.Dtos.Generation;
using MediatR;

namespace BaseFaq.AI.Generation.Business.Generation.Commands.RequestGeneration;

public sealed record GenerationRequestCommand(
    GenerationRequestDto Request,
    string? IdempotencyKey) : IRequest<GenerationRequestAcceptedResponse>;