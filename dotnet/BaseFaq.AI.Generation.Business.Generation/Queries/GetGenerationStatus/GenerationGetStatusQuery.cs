using BaseFaq.Models.Ai.Dtos.Generation;
using MediatR;

namespace BaseFaq.AI.Generation.Business.Generation.Queries.GetGenerationStatus;

public sealed record GenerationGetStatusQuery : IRequest<GenerationStatusResponse>;