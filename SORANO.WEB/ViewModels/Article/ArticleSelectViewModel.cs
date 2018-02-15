using Newtonsoft.Json;

namespace SORANO.WEB.ViewModels.Article
{
    public class ArticleSelectViewModel
    {
        [JsonProperty(PropertyName = "id")]
        public int ID { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }

        [JsonProperty(PropertyName = "barcode")]
        public string Barcode { get; set; }
    }
}