using SORANO.CORE.StockEntities;
using SORANO.DAL.Context;
using SORANO.DAL.Repositories.Abstract;

namespace SORANO.DAL.Repositories
{
    /// <summary>
    /// Generic repository for attachment types
    /// </summary>
    public class AttachmentTypeRepository : StockEntityRepository<AttachmentType>, IAttachmentTypeRepository
    {
        /// <summary>
        /// Generic repository for attachment types
        /// </summary>
        /// <param name="context">Data context</param>
        public AttachmentTypeRepository(StockContext context) : base(context)
        {            
        }
    }
}