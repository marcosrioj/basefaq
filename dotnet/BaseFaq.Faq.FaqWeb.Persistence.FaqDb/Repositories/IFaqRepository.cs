using BaseFaq.Common.EntityFramework.Core.Repositories;

namespace BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Repositories;

public interface IFaqRepository : IBaseRepository<FaqWeb.Persistence.FaqDb.Entities.Faq>
{
}