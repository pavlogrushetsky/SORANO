using System;
using System.Linq;
using SORANO.BLL.Services.Abstract;
using SORANO.DAL.Repositories;
using System.Threading.Tasks;
using SORANO.BLL.Helpers;
using SORANO.CORE.StockEntities;
using System.Collections.Generic;

namespace SORANO.BLL.Services
{
    public class GoodsService : BaseService, IGoodsService
    {
        public GoodsService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task ChangeLocationAsync(int articleId, int currentLocationId, int targetLocationId, int num, int userId)
        {
            var storages = await _unitOfWork.Get<Storage>().GetAllAsync();

            var currentStorages = storages
                .Where(s => s.LocationID == currentLocationId && s.Goods.DeliveryItem.ArticleID == articleId && !s.ToDate.HasValue)
                .Take(num)
                .ToList();

            currentStorages.ForEach(storage =>
            {
                storage.ToDate = DateTime.Now;
                storage.UpdateModifiedFields(userId);

                _unitOfWork.Get<Storage>().Update(storage);
            });

            currentStorages.Select(s => s.Goods).ToList().ForEach(goods =>
            {
                var storage = new Storage
                {
                    LocationID = targetLocationId,
                    FromDate = DateTime.Now
                };

                storage.UpdateCreatedFields(userId).UpdateModifiedFields(userId);

                goods.Storages.Add(storage);

                goods.UpdateModifiedFields(userId);

                _unitOfWork.Get<Goods>().Update(goods);
            });
            
            await _unitOfWork.SaveAsync();
        }

        public async Task<List<Article>> GetArticlesForLocationAsync(int? locationId)
        {
            if (!locationId.HasValue || locationId == 0)
            {
                var goods = await _unitOfWork.Get<Goods>().GetAllAsync();

                return goods.Select(g => g.DeliveryItem.Article).ToList();
            }

            var location = await _unitOfWork.Get<Location>().GetAsync(locationId.Value);

            return location.Storages.Where(s => !s.ToDate.HasValue).Select(s => s.Goods.DeliveryItem.Article).ToList();
        }

        public async Task<List<Goods>> GetSoldGoodsAsync()
        {
            var goods = await _unitOfWork.Get<Goods>().FindByAsync(g => g.SaleDate.HasValue && g.SaleLocationID.HasValue && g.SalePrice.HasValue);

            return goods.ToList();
        }

        public async Task<decimal> GetTotalIncomeAsync()
        {
            var goods = await _unitOfWork.Get<Goods>().FindByAsync(g => g.SaleDate.HasValue && g.SaleLocationID.HasValue && g.SalePrice.HasValue);

            return goods.Sum(g => g.SalePrice.Value);
        }

        public async Task SaleAsync(int articleId, int locationId, int clientId, int num, decimal price, int userId)
        {
            var storages = await _unitOfWork.Get<Storage>().GetAllAsync();

            var currentStorages = storages
                .Where(s => s.LocationID == locationId && s.Goods.DeliveryItem.ArticleID == articleId && !s.ToDate.HasValue)
                .Take(num)
                .ToList();

            currentStorages.ForEach(storage =>
            {
                storage.ToDate = DateTime.Now.Date;
                storage.UpdateModifiedFields(userId);

                _unitOfWork.Get<Storage>().Update(storage);
            });

            currentStorages.Select(s => s.Goods).ToList().ForEach(goods =>
            {
                goods.SaleDate = DateTime.Now;
                goods.SaleLocationID = locationId;
                goods.SoldBy = userId;
                goods.SalePrice = price;
                goods.ClientID = clientId;

                goods.UpdateModifiedFields(userId);

                _unitOfWork.Get<Goods>().Update(goods);
            });

            await _unitOfWork.SaveAsync();
        }
    }
}
