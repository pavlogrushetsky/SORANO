using System;
using System.Linq;
using SORANO.CORE.IdentityEntities;

namespace SORANO.DAL.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<Context.StockContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(Context.StockContext context)
        {
            if (!context.Users.Any())
            {
                context.Users.Add(new User
                {
                    Name = "Павел Грушецкий",
                    Email = "pavlo.grushetsky@gmail.com",
                    Password = "sQnzu7wkTrgkQZF+0G1hi5AI3Qmzvv0bXgc5THBqi7mAsdd4Xll27ASbRt9fEyavWi6m0QP9B8lThf+rDKy8hg==",
                    IsBlocked = false,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now
                });
            }

            base.Seed(context);
        }
    }
}
