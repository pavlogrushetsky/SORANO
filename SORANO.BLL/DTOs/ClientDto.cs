namespace SORANO.BLL.Dtos
{
    public class ClientDto : BaseDto
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string PhoneNumber { get; set; }

        public string CardNumber { get; set; }
    }
}