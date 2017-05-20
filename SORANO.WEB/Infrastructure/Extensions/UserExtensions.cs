using Microsoft.AspNetCore.Http;
using SORANO.CORE.AccountEntities;
using SORANO.WEB.Models.User;
using System.Collections.Generic;
using System.Globalization;
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
        /// <param name="hasActivities">Specifies if passed user has any activities</param>
        /// <returns></returns>
        public static UserListModel ToListModel(this User user, bool isCurrent = false, bool hasActivities = false)
        {
            return new UserListModel
            {
                ID = user.ID,
                Login = user.Login,
                Description = user.Description,                               
                Roles = user.Roles?.Select(r => r.Description).ToList(),
                IsBlocked = user.IsBlocked,
                CanBeDeleted = !(isCurrent || hasActivities),
                CanBeBlocked = !isCurrent
            };
        }

        public static UserUpdateModel ToUpdateModel(this User user, bool canBeModified = true)
        {
            return new UserUpdateModel
            {
                ID = user.ID,
                Login = user.Login,
                Description = user.Description,
                Roles = user.Roles?.Select(r => r.Name).ToList(),
                CanBeModified = canBeModified
            };
        }

        public static void FromUpdateModel(this User user, UserUpdateModel model, List<Role> roles, bool canBeModified = true)
        {
            
            user.Description = model.Description;

            if (!canBeModified)
            {
                return;
            }

            user.Login = model.Login;

            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                user.Password = model.NewPassword;
            }

            user.Roles
                .Where(r => !model.Roles.Contains(r.Name))
                .ToList()
                .ForEach(r => user.Roles.Remove(r));

            var existentRoles = user.Roles;

            roles
                .Where(r => model.Roles.Contains(r.Name))
                .Except(existentRoles)
                .ToList()
                .ForEach(r => user.Roles.Add(r));
        }

        public static void FromCreateModel(this User user, UserCreateModel model, List<Role> roles)
        {
            user.Login = model.Login;
            user.Description = model.Description;
            user.Password = model.Password;

            roles
                .Where(r => model.Roles.Select(x => x.Name).Contains(r.Name))
                .ToList()
                .ForEach(r => user.Roles.Add(r));
        }

        /// <summary>
        /// Convert user entity to user delete model
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="IsCurrent">Specifies if passed user is current logged in user</param>
        /// <param name="hasActivities">Specifies if passed user has any activities</param>
        /// <returns></returns>
        public static UserDeleteModel ToDeleteModel(this User user, bool IsCurrent = false, bool hasActivities = false)
        {
            return new UserDeleteModel
            {
                ID = user.ID,
                Login = user.Login,
                Description = user.Description,
                CanBeDeleted = !(IsCurrent || hasActivities)
            };
        }

        /// <summary>
        /// Convert user entity to user block model
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="IsCurrent">Specifies if passed user is current logged in user</param>
        /// <returns></returns>
        public static UserBlockModel ToBlockModel(this User user, bool IsCurrent = false)
        {
            return new UserBlockModel
            {
                ID = user.ID,
                Login = user.Login,
                Description = user.Description,
                CanBeBlocked = !IsCurrent,
                IsBlocked = user.IsBlocked
            };
        }

        public static UserDetailsModel ToDetailsModel(this User user, bool isCurrent = false, bool hasActivities = false)
        {
            var model = new UserDetailsModel
            {
                ID = user.ID,
                Login = user.Login,
                Description = user.Description,
                Roles = user.Roles?.Select(r => r.Description).ToList(),
                IsBlocked = user.IsBlocked,
                CanBeDeleted = !(isCurrent || hasActivities),
                CanBeBlocked = !isCurrent,
                Sales = user.SoldGoods?.Select(g => new UserSaleModel
                {
                    ArticleName = g.DeliveryItem.Article.Name,
                    Location = g.SaleLocation.Name,
                    Price = g.SalePrice?.ToString("C", new CultureInfo("uk-UA")),
                    Date = g.SaleDate?.ToString("dd.MM.yyyy")
                }).ToList()
            };

            model.Activities = new List<UserActivityModel>();
            if (user.CreatedEntities != null)
                model.Activities.AddRange(user.CreatedEntities.Select(e => new UserActivityModel
                {
                    ActivityType = "Создание",
                    EntityID = e.ID,
                    EntityType = e.GetType().Name,
                    Date = e.CreatedDate.ToString("dd.MM.yyyy")
                }));
            if (user.ModifiedEntities != null)
                model.Activities.AddRange(user.ModifiedEntities?.Select(e => new UserActivityModel
                {
                    ActivityType = "Редактирование",
                    EntityID = e.ID,
                    EntityType = e.GetType().Name,
                    Date = e.CreatedDate.ToString("dd.MM.yyyy")
                }));
            if (user.DeletedEntities != null)
                model.Activities.AddRange(user.DeletedEntities?.Select(e => new UserActivityModel
                {
                    ActivityType = "Удаление",
                    EntityID = e.ID,
                    EntityType = e.GetType().Name,
                    Date = e.CreatedDate.ToString("dd.MM.yyyy")
                }));

            return model;
        }

        /// <summary>
        /// Check if user has any activities
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>True if user has activities else false</returns>
        public static bool HasActivities(this User user)
        {
            return user.ModifiedEntities.Any()
                || user.CreatedEntities.Any()
                || user.DeletedEntities.Any()
                || user.SoldGoods.Any();
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
