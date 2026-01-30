using BaseFaq.Common.Infrastructure.Core.Abstractions;
using BaseFaq.Faq.Business.Faq.Abstractions;
using BaseFaq.Faq.Business.Faq.Commands.CreateFaq;
using BaseFaq.Models.Faq.Dtos.Faq;
using MediatR;

namespace BaseFaq.Faq.Business.Faq.Service;

public class FaqService(IMediator mediator, ISessionService sessionService) : IFaqService
{
    public async Task<FaqDto> Create(FaqCreateRequestDto requestDto, CancellationToken token)
    {
        var command = new FaqsCreateFaqCommand
        {
            Name = requestDto.Name,
            Language = requestDto.Language,
            Status = requestDto.Status,
            SortType = requestDto.SortType,
            TenantId = sessionService.TenantId!.Value
        };

        var id = await mediator.Send(command, token);

        //TODO
        //var query = new FaqsGetFaqQuery { Id = id };

        //var result = await mediator.Send(query, token);

        //await PublishCouchbaseSync(feedlotId, id, DataSyncEventAction.LotCreate, token);

        //return result;

        return null;
    }
}