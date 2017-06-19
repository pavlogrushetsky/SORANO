using Microsoft.AspNetCore.Http;
using SORANO.CORE.AccountEntities;
using SORANO.WEB.Models.User;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

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
                Roles = user.Roles?.Select(r => r.ToModel()).ToList(),
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

        public static void FromCreateModel(this User user, UserModel model, List<Role> roles)
        {
            user.Login = model.Login;
            user.Description = model.Description;
            user.Password = model.Password;

            roles
                .Where(r => model.Roles.Select(x => x.Name).Contains(r.Name))
                .ToList()
                .ForEach(r => user.Roles.Add(r));
        }

        public static User ToEntity(this UserModel model)
        {
            return new User
            {
                ID = model.ID,
                Login = model.Login,
                Description = model.Description,
                Password = model.Password,
                Roles = model.Roles.Select(r => r.ToEntity()).ToList()
            };
        }

        /// <summary>
        /// Check if user is currently logged in user
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="context">HttpContext</param>
        /// <returns>True if user is currently logged in user else false</returns>
        public static bool IsCurrent(this User user, HttpContext context)
        {
            return context.User.FindFirst(ClaimTypes.Name)?.Value?.Equals(user.Login) ?? false;
        }
    }
}
