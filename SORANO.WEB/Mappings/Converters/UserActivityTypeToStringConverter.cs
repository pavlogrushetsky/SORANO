using System;
using AutoMapper;
using SORANO.BLL.Dtos;

namespace SORANO.WEB.Mappings.Converters
{
    public class UserActivityTypeToStringConverter : ITypeConverter<UserActivityType, string>
    {
        public string Convert(UserActivityType source, string destination, ResolutionContext context)
        {
            switch (source)
            {
                case UserActivityType.Creation:
                    return "Создание";
                case UserActivityType.Updating:
                    return "Обновление";
                case UserActivityType.Deletion:
                    return "Удаление";
                default:
                    throw new ArgumentException(nameof(source));
            }
        }
    }
}