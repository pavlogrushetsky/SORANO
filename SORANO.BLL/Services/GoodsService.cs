using System;
using System.Linq;
using SORANO.BLL.Services.Abstract;
using SORANO.DAL.Repositories;
using System.Threading.Tasks;
using SORANO.CORE.StockEntities;
using System.Collections.Generic;
using SORANO.BLL.Dtos;
using SORANO.BLL.Extensions;

namespace SORANO.BLL.Services
{
    public class GoodsService : BaseService, IGoodsService
    {
        public GoodsService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<ServiceResponse<int>> ChangeLocationAsync(int articleId, int currentLocationId, int targetLocationId, int num, int userId)
        {
            var storages = await UnitOfWork.Get<Storage>().GetAllAsync();

            var currentStorages = storages
                .Where(s => s.LocationID == currentLocationId && s.Goods.DeliveryItem.ArticleID == articleId && !s.ToDate.HasValue)
                .Take(num)
                .ToList();

            currentStorages.ForEach(storage =>
            {
                storage.ToDate = DateTime.Now;
                storage.UpdateModifiedFields(userId);

                UnitOfWork.Get<Storage>().Update(storage);
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

                UnitOfWork.Get<Goods>().Update(goods);
            });
            
            await UnitOfWork.SaveAsync();

            return new SuccessResponse<int>(targetLocationId);
        }

        public async Task<ServiceResponse<IEnumerable<GoodsDto>>> GetAllAsync()
        {
            var response = new SuccessResponse<IEnumerable<GoodsDto>>();

            var goods = await UnitOfWork.Get<Goods>().GetAllAsync();

            response.Result = goods.Select(a => a.ToDto());

            return response;
        }

        public async Task<ServiceResponse<IEnumerable<GoodsDto>>> GetSoldGoodsAsync()
        {
            var goods = await UnitOfWork.Get<Goods>().FindByAsync(g => g.SaleDate.HasValue && g.SaleLocationID.HasValue && g.SalePrice.HasValue);

            return new SuccessResponse<IEnumerable<GoodsDto>>(goods.Select(g => g.ToDto()));
        }

        public async Task<ServiceResponse<decimal>> GetTotalIncomeAsync()
        {
            var goods = await UnitOfWork.Get<Goods>().FindByAsync(g => g.SaleDate.HasValue && g.SaleLocationID.HasValue && g.SalePrice.HasValue);

            var sum = goods?.Sum(g => g.SalePrice) ?? 0.0M;

            return new SuccessResponse<decimal>(sum);
        }

        public async Task<ServiceResponse<int>> SaleAsync(int articleId, int locationId, int clientId, int num, decimal price, int userId)
        {
            var storages = await UnitOfWork.Get<Storage>().GetAllAsync();

            var currentStorages = storages
                .Where(s => s.LocationID == locationId && s.Goods.DeliveryItem.ArticleID == articleId && !s.ToDate.HasValue)
                .Take(num)
                .ToList();

            currentStorages.ForEach(storage =>
            {
                storage.ToDate = DateTime.Now.Date;
                storage.UpdateModifiedFields(userId);

                UnitOfWork.Get<Storage>().Update(storage);
            });

            currentStorages.Select(s => s.Goods).ToList().ForEach(goods =>
            {
                goods.SaleDate = DateTime.Now;
                goods.SaleLocationID = locationId;
                goods.SoldBy = userId;
                goods.SalePrice = price;
                goods.ClientID = clientId;

                goods.UpdateModifiedFields(userId);

                UnitOfWork.Get<Goods>().Update(goods);
            });

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<int>(num);
        }
    }
}
