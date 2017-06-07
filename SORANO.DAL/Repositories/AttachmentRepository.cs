using SORANO.CORE.StockEntities;
using SORANO.DAL.Context;
using SORANO.DAL.Repositories.Abstract;

namespace SORANO.DAL.Repositories
{
    /// <summary>
    /// Generic repository for attachments
    /// </summary>
    public class AttachmentRepository : EntityRepository<Attachment>, IAttachmentRepository
    {
        /// <summary>
        /// Generic repository for attachments
        /// </summary>
        /// <param name="factory">Context factory</param>
        public AttachmentRepository(IStockFactory factory) : base(factory)
        {
        }
    }
}