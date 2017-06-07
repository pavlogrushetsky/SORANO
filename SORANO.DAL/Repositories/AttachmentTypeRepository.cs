using SORANO.CORE.StockEntities;
using SORANO.DAL.Context;
using SORANO.DAL.Repositories.Abstract;

namespace SORANO.DAL.Repositories
{
    /// <summary>
    /// Generic repository for attachment types
    /// </summary>
    public class AttachmentTypeRepository : EntityRepository<AttachmentType>, IAttachmentTypeRepository
    {
        /// <summary>
        /// Generic repository for attachment types
        /// </summary>
        /// <param name="factory">Context factory</param>
        public AttachmentTypeRepository(IStockFactory factory) : base(factory)
        {            
        }
    }
}