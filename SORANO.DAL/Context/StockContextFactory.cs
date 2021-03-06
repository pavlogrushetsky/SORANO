﻿using System.Data.Entity.Infrastructure;

namespace SORANO.DAL.Context
{
    public class StockContextFactory : IDbContextFactory<StockContext>
    {
        public static string ConnectionString { get; set; } = "Server=GRUSHETSKY-PC;Database=SORANO_DEV_2;Trusted_Connection=True;MultipleActiveResultSets=true";

        public StockContext Create()
        {           
            return new StockContext(ConnectionString);
        }
    }
}