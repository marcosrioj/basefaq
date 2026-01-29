using BaseFaq.Common.EntityFramework.Core.Repositories;
using BaseFaq.Faq.Persistence.FaqDb.Entities;

namespace BaseFaq.Faq.Persistence.FaqDb.Repositories;

public sealed class FaqRepository : BaseRepository<FaqDbContext, Entities.Faq>, IFaqRepository
{
    public FaqRepository(FaqDbContext context)
        : base(context)
    {
    }
}