using System.Collections.Generic;
using System.Linq;
using SORANO.BLL.Dtos;
using SORANO.CORE.AccountEntities;

namespace SORANO.BLL.Extensions
{
    internal static class UserExtensions
    {
        public static UserDto ToDto(this User model)
        {
            var dto = new UserDto
            {
                ID = model.ID,
                Login = model.Login,
                Description = model.Description,
                IsBlocked = model.IsBlocked,
                Roles = model.Roles.Select(r => r.ToDto())
            };

            var activities = new List<UserActivityDto>();
            activities.AddRange(model.CreatedEntities.Select(e => e.ToUserActivityDto(UserActivityType.Creation)));
            activities.AddRange(model.ModifiedEntities.Select(e => e.ToUserActivityDto(UserActivityType.Updating)));
            activities.AddRange(model.DeletedEntities.Select(e => e.ToUserActivityDto(UserActivityType.Deletion)));

            dto.Activities = activities;

            return dto;
        }

        public static User ToEntity(this UserDto dto)
        {
            return new User
            {
                ID = dto.ID,
                Login = dto.Login,
                Description = dto.Description,
                Password = dto.Password,
                Roles = dto.Roles.Select(r => r.ToEntity()).ToList()
            };
        }
    }
}