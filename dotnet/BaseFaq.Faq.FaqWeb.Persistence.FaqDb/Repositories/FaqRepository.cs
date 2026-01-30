using BaseFaq.Common.EntityFramework.Core.Repositories;

namespace BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Repositories;

public sealed class FaqRepository : BaseRepository<FaqDbContext, FaqWeb.Persistence.FaqDb.Entities.Faq>, IFaqRepository
{
    public FaqRepository(FaqDbContext context)
        : base(context)
    {
    }
}