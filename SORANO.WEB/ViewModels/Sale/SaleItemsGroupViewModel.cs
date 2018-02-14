using System.Collections.Generic;

namespace SORANO.WEB.ViewModels.Sale
{
    public class SaleItemsGroupViewModel
    {
        public string ArticleName { get; set; }

        public string ArticleTypeName { get; set; }

        public string Price { get; set; }

        public int Count { get; set; }

        public int SelectedCount { get; set; }

        public bool IsSelected { get; set; }

        public bool HasMainPicture => !string.IsNullOrWhiteSpace(MainPicturePath);

        public string MainPicturePath { get; set; }

        public string GoodsIds { get; set; }

        public List<SaleItemViewModel> Items { get; set; } = new List<SaleItemViewModel>();
    }
}