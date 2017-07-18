﻿using SORANO.CORE.AccountEntities;
using System.Linq;
using SORANO.WEB.Models;

namespace SORANO.WEB.Infrastructure.Extensions
{
    public static class UserExtensions
    {
        /// <summary>
        /// Convert user entity to user list model to display for list of users
        /// </summary>
        /// <param name="user">User entity</param>
        /// <param name="isCurrent">Specifies if passed user is current logged in user</param>
        /// <returns></returns>
        public static UserModel ToModel(this User user, bool isCurrent = false)
        {
            var hasActivities = user.CreatedEntities.Any()
                                || user.ModifiedEntities.Any()
                                || user.DeletedEntities.Any();

            var model = new UserModel
            {
                ID = user.ID,
                Login = user.Login,
                Description = user.Description,                               
                Roles = user.Roles?.Select(r => r.Description).ToList(),
                RoleIDs = user.Roles?.Select(r => r.ID.ToString()),
                IsBlocked = user.IsBlocked,
                CanBeDeleted = !(isCurrent || hasActivities),
                CanBeBlocked = !isCurrent,
                CanBeModified = !isCurrent,
                Sales = user.SoldGoods?.Select(g => g.ToUserSaleModel()).ToList()
            };

            if (user.CreatedEntities.Any())
            {
                model.Activities.AddRange(user.CreatedEntities.Select(e => e.ToUserActivityModel("Создание")));
            }
            if (user.ModifiedEntities.Any())
            {
                model.Activities.AddRange(user.ModifiedEntities.Select(e => e.ToUserActivityModel("Редактирование")));
            }
            if (user.DeletedEntities.Any())
            {
                model.Activities.AddRange(user.DeletedEntities.Select(e => e.ToUserActivityModel("Удаление")));
            }

            return model;
        }       

        public static User ToEntity(this UserModel model)
        {
            return new User
            {
                ID = model.ID,
                Login = model.Login,
                Description = model.Description,
                Password = model.NewPassword ?? model.Password,
                Roles = model.RoleIDs.Select(r => new Role
                {
                    ID = int.Parse(r)
                }).ToList()
            };
        }
    }
}
