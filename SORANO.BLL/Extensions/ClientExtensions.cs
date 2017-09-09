using SORANO.BLL.Dtos;
using SORANO.CORE.StockEntities;

namespace SORANO.BLL.Extensions
{
    internal static class ClientExtensions
    {
        public static ClientDto ToDto(this Client model)
        {
            return new ClientDto
            {

            };
        }

        public static Client ToEntity(this ClientDto dto)
        {
            return new Client
            {

            };
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
