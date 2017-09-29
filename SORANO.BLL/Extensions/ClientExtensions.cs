using System.Linq;
using SORANO.BLL.Dtos;
using SORANO.CORE.StockEntities;

namespace SORANO.BLL.Extensions
{
    internal static class ClientExtensions
    {
        public static ClientDto ToDto(this Client model)
        {
            var dto = new ClientDto
            {
                ID = model.ID,
                Name = model.Name,
                Description = model.Description,
                PhoneNumber = model.PhoneNumber,
                CardNumber = model.CardNumber
            };

            dto.MapDetails(model);
            dto.CanBeDeleted = !model.Goods.Any() && !model.IsDeleted;

            return dto;
        }

        public static Client ToEntity(this ClientDto dto)
        {
            var entity = new Client
            {
                ID = dto.ID,
                Name = dto.Name,
                PhoneNumber = dto.PhoneNumber,
                CardNumber = dto.CardNumber,
                Description = dto.Description,
                Recommendations = dto.Recommendations.Select(r => r.ToEntity()).ToList(),
                Attachments = dto.Attachments.Select(a => a.ToEntity()).ToList()
            };

            if (!string.IsNullOrEmpty(dto.MainPicture?.FullPath))
                entity.Attachments.Add(dto.MainPicture.ToEntity());

            return entity;
        }

        public static void UpdateFields(this Client existentClient, Client newClient)
        {
            existentClient.Name = newClient.Name;
            existentClient.Description = newClient.Description;
            existentClient.PhoneNumber = newClient.PhoneNumber;
            existentClient.CardNumber = newClient.CardNumber;
        }
    }
}
