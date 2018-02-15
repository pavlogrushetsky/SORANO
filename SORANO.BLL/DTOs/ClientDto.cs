namespace SORANO.BLL.Dtos
{
    public class ClientDto : BaseDto
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string PhoneNumber { get; set; }

        public string CardNumber { get; set; }
    }
}