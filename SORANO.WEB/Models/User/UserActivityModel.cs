namespace SORANO.WEB.Models.User
{
    public class UserActivityModel
    {
        public string ActivityType { get; set; }

        public int EntityID { get; set; }

        public string EntityType { get; set; }

        public string Date { get; set; }
    }
}
