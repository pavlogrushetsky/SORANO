﻿using System;
using System.Data.Entity.Migrations;
using System.Linq;
using SORANO.CORE.AccountEntities;
using SORANO.CORE.StockEntities;
using SORANO.DAL.Context;

namespace SORANO.WEB.Migrations
{
    public class Configuration : DbMigrationsConfiguration<StockContext>
	{
	    public Configuration()
	    {
	        AutomaticMigrationsEnabled = true;
	        AutomaticMigrationDataLossAllowed = true;
	    }

	    protected override void Seed(StockContext context)
	    {
            if (!context.Users.Any() && !context.Roles.Any())
            {
                var mainDeveloper = context.Users.Add(new User
                {
                    Description = "Pavlo Grushetsky",
                    Login = "pavlo.grushetsky",
                    Password = "sQnzu7wkTrgkQZF+0G1hi5AI3Qmzvv0bXgc5THBqi7mAsdd4Xll27ASbRt9fEyavWi6m0QP9B8lThf+rDKy8hg==",
                    IsBlocked = false
                });

                mainDeveloper.Roles.Add(new Role
                {
                    Name = "developer",
                    Description = "Developer"
                });

                mainDeveloper.Roles.Add(new Role
                {
                    Name = "administrator",
                    Description = "Administrator"
                });

                mainDeveloper.Roles.Add(new Role
                {
                    Name = "editor",
                    Description = "Editor"
                });

                mainDeveloper.Roles.Add(new Role
                {
                    Name = "manager",
                    Description = "Manager"
                });

                mainDeveloper.Roles.Add(new Role
                {
                    Name = "user",
                    Description = "User"
                });

                context.SaveChanges();
            }

            if (!context.AttachmentTypes.Any(a => a.Name.Equals("Основное изображение")))
            {
                var developer = context.Users.First(u => u.Roles.Select(r => r.Name).Contains("developer"));

                context.AttachmentTypes.Add(new AttachmentType
                {
                    Name = "Основное изображение",
                    Comment = "Изображение для отображения на странице детальной информации",
                    Extensions = "png,bmp,dwg,gif,ico,jpeg,jpg,pic,tif,tiff",
                    CreatedDate = DateTime.Now,
                    CreatedBy = developer.ID,
                    ModifiedDate = DateTime.Now,
                    ModifiedBy = developer.ID
                });

                context.SaveChanges();
            }

            base.Seed(context);
        }
	}
}
