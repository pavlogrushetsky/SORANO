using SORANO.CORE.StockEntities;
using System.Linq;
using SORANO.WEB.Models;

namespace SORANO.WEB.Infrastructure.Extensions
{
    public static class ClientExtensions
    {
        public static ClientModel ToModel(this Client client)
        {
            return new ClientModel
            {
                ID = client.ID,
                Name = client.Name,
                Description = client.Description,
                PhoneNumber = client.PhoneNumber,
                CardNumber = client.CardNumber,
                Recommendations = client.Recommendations?.Where(r => !r.IsDeleted).Select(r => r.ToModel()).ToList(),
                MainPicture = client.Attachments?.SingleOrDefault(a => !a.IsDeleted && a.Type.Name.Equals("Основное изображение"))?.ToModel() ?? new AttachmentModel(),
                Attachments = client.Attachments?.Where(a => !a.IsDeleted && !a.Type.Name.Equals("Основное изображение")).Select(a => a.ToModel()).ToList(),
                CanBeDeleted = !client.Goods.Any() && !client.IsDeleted,
                IsDeleted = client.IsDeleted,
                Created = client.CreatedDate.ToString("dd.MM.yyyy"),
                Modified = client.ModifiedDate.ToString("dd.MM.yyyy"),
                Deleted = client.DeletedDate?.ToString("dd.MM.yyyy"),
                CreatedBy = client.CreatedByUser?.Login,
                ModifiedBy = client.ModifiedByUser?.Login,
                DeletedBy = client.DeletedByUser?.Login
            };
        }

        public static Client ToEntity(this ClientModel model)
        {
            var client = new Client
            {
                ID = model.ID,
                Name = model.Name,
                Description = model.Description,
                PhoneNumber = model.PhoneNumber,
                CardNumber = model.CardNumber,
                Recommendations = model.Recommendations.Select(r => r.ToEntity()).ToList(),
                Attachments = model.Attachments.Select(a => a.ToEntity()).ToList()
            };

            if (!string.IsNullOrEmpty(model.MainPicture?.Name))
            {
                client.Attachments.Add(model.MainPicture.ToEntity());
            }

            return client;
        }
    }
}
