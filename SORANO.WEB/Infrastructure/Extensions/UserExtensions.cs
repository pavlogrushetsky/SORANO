using Microsoft.AspNetCore.Http;
using SORANO.CORE.AccountEntities;
using SORANO.WEB.Models.User;
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
