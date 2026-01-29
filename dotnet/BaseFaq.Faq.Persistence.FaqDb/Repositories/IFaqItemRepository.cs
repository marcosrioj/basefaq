using BaseFaq.Common.EntityFramework.Core.Repositories;
using BaseFaq.Faq.Persistence.FaqDb.Entities;

namespace BaseFaq.Faq.Persistence.FaqDb.Repositories;

public interface IFaqItemRepository : IBaseRepository<FaqItem>
{
}