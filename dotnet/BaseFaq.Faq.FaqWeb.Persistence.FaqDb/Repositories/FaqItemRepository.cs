using BaseFaq.Common.EntityFramework.Core.Repositories;
using BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Entities;

namespace BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Repositories;

public sealed class FaqItemRepository : BaseRepository<FaqDbContext, FaqItem>, IFaqItemRepository
{
    public FaqItemRepository(FaqDbContext context)
        : base(context)
    {
    }
}