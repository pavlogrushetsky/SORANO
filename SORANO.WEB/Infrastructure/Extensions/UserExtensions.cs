using SORANO.CORE.AccountEntities;
using System.Linq;
using SORANO.WEB.ViewModels;

namespace SORANO.WEB.Infrastructure.Extensions
{
    public static class UserExtensions
    {
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
