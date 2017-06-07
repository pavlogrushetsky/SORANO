using System;

namespace SORANO.DAL.Context
{
    public interface IStockFactory : IDisposable
    {
        /// <summary>
        /// Initialize data context
        /// </summary>
        /// <returns>Data context</returns>
        StockContext Init();
    }
}
