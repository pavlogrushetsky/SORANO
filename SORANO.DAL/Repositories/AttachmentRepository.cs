using SORANO.CORE.StockEntities;
using SORANO.DAL.Context;

namespace SORANO.DAL.Repositories
{
    /// <summary>
    /// Generic repository for attachments
    /// </summary>
    public class AttachmentRepository : StockEntityRepository<Attachment>
    {
        /// <summary>
        /// Generic repository for attachments
        /// </summary>
        /// <param name="context">Data context</param>
        public AttachmentRepository(StockContext context) : base(context)
        {
        }
    }
}