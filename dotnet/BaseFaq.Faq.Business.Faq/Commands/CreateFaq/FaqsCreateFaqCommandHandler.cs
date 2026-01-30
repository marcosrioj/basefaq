using BaseFaq.Faq.Persistence.FaqDb.Repositories;
using MediatR;

namespace BaseFaq.Faq.Business.Faq.Commands.CreateFaq;

public class FaqsCreateFaqCommandHandler(IFaqRepository faqRepository) : IRequestHandler<FaqsCreateFaqCommand, Guid>
{
    public Task<Guid> Handle(FaqsCreateFaqCommand request, CancellationToken cancellationToken)
    {
        //TODO Create a Faq with faqRepository
        throw new NotImplementedException();
    }
}