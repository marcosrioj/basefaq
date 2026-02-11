using BaseFaq.Common.Infrastructure.ApiErrorHandling.Exception;
using BaseFaq.Faq.Common.Persistence.FaqDb;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BaseFaq.Faq.FaqWeb.Business.Faq.Commands.DeleteFaqContentRef;

public class FaqContentRefsDeleteFaqContentRefCommandHandler(FaqDbContext dbContext)
    : IRequestHandler<FaqContentRefsDeleteFaqContentRefCommand>
{
    public async Task Handle(FaqContentRefsDeleteFaqContentRefCommand request, CancellationToken cancellationToken)
    {
        var faqContentRef = await dbContext.FaqContentRefs
            .FirstOrDefaultAsync(entity => entity.Id == request.Id, cancellationToken);
        if (faqContentRef is null)
        {
            throw new ApiErrorException(
                $"FAQ content ref '{request.Id}' was not found.",
                errorCode: (int)HttpStatusCode.NotFound);
        }

        dbContext.FaqContentRefs.Remove(faqContentRef);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}