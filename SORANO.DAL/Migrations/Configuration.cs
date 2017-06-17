using System.Linq;
using SORANO.CORE.AccountEntities;

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
            if (!context.Users.Any() && !context.Roles.Any())
            {
                var developer = context.Users.Add(new User
                {
                    Description = "Pavlo Grushetsky",
                    Login = "pavlo.grushetsky",
                    Password = "sQnzu7wkTrgkQZF+0G1hi5AI3Qmzvv0bXgc5THBqi7mAsdd4Xll27ASbRt9fEyavWi6m0QP9B8lThf+rDKy8hg==",
                    IsBlocked = false
                });

                developer.Roles.Add(new Role
                {
                    Name = "developer",
                    Description = "Developer"
                });

                developer.Roles.Add(new Role
                {
                    Name = "administrator",
                    Description = "Administrator"
                });

                developer.Roles.Add(new Role
                {
                    Name = "editor",
                    Description = "Editor"
                });

                developer.Roles.Add(new Role
                {
                    Name = "manager",
                    Description = "Manager"
                });

                developer.Roles.Add(new Role
                {
                    Name = "user",
                    Description = "User"
                });
            }

            base.Seed(context);
        }
    }
}
