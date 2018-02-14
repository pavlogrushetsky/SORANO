using SORANO.BLL.Dtos;
using SORANO.BLL.Extensions;
using SORANO.BLL.Services.Abstract;
using SORANO.CORE.StockEntities;
using SORANO.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SORANO.BLL.Services
{
    public class SaleService : BaseService, ISaleService
    {
        public SaleService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }                             

        #region CRUD Methods

        public async Task<ServiceResponse<IEnumerable<SaleDto>>> GetAllAsync(bool withDeleted)
        {
            var response = new SuccessResponse<IEnumerable<SaleDto>>();

            var sales = await UnitOfWork.Get<Sale>().GetAllAsync();

            response.Result = !withDeleted
                ? sales.Where(s => !s.IsDeleted).Select(s => s.ToDto())
                : sales.Select(s => s.ToDto());

            return response;
        }

        public async Task<ServiceResponse<SaleDto>> GetAsync(int id)
        {
            var sale = await UnitOfWork.Get<Sale>().GetAsync(id);

            return sale == null
                ? new ServiceResponse<SaleDto>(ServiceResponseStatus.NotFound)
                : new SuccessResponse<SaleDto>(sale.ToDto());
        }

        public async Task<ServiceResponse<int>> CreateAsync(SaleDto sale, int userId)
        {
            if (sale == null)
                throw new ArgumentNullException(nameof(sale));

            var entity = sale.ToEntity();

            entity.UpdateCreatedFields(userId).UpdateModifiedFields(userId);

            var location = await UnitOfWork.Get<Location>().GetAsync(entity.LocationID);
            location.UpdateModifiedFields(userId);

            if (entity.ClientID.HasValue)
            {
                var client = await UnitOfWork.Get<Client>().GetAsync(entity.ClientID.Value);
                client.UpdateModifiedFields(userId);
            }

            entity.Recommendations?.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
            entity.Attachments?.UpdateCreatedFields(userId).UpdateModifiedFields(userId);

            var added = UnitOfWork.Get<Sale>().Add(entity);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<int>(added.ID);

        }

        public Task<ServiceResponse<SaleDto>> UpdateAsync(SaleDto sale, int userId)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<int>> DeleteAsync(int id, int userId)
        {
            var existentSale = await UnitOfWork.Get<Sale>().GetAsync(id);

            if (existentSale == null)
                return new ServiceResponse<int>(ServiceResponseStatus.NotFound);

            if (existentSale.IsSubmitted)
                return new ServiceResponse<int>(ServiceResponseStatus.InvalidOperation);

            var saleGoodsIds = existentSale.Goods.Select(g => g.ID);
            var goods = await UnitOfWork.Get<Goods>().FindByAsync(g => saleGoodsIds.Contains(g.ID));

            goods.ToList().ForEach(g =>
            {
                g.SaleID = null;
                g.Price = null;
                g.UpdateModifiedFields(userId);
                UnitOfWork.Get<Goods>().Update(g);
            });

            UnitOfWork.Get<Sale>().Delete(existentSale);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<int>(id);
        }

        #endregion

        public async Task<ServiceResponse<int>> GetUnsubmittedCountAsync(int userId, int? locationId)
        {
            var deliveries = await UnitOfWork.Get<Sale>().FindByAsync(s => !s.IsSubmitted && !s.IsDeleted && s.UserID == userId && (!locationId.HasValue || s.LocationID == locationId.Value));

            return new SuccessResponse<int>(deliveries?.Count() ?? 0);
        }

        public async Task<ServiceResponse<IEnumerable<SaleDto>>> GetAllAsync(bool withDeleted, int userId, int? locationId)
        {
            var response = new SuccessResponse<IEnumerable<SaleDto>>();

            var sales = await UnitOfWork.Get<Sale>().FindByAsync(s => s.UserID == userId && (!locationId.HasValue || s.LocationID == locationId.Value));

            response.Result = !withDeleted
                ? sales.Where(s => !s.IsDeleted).Select(s => s.ToDto())
                : sales.Select(s => s.ToDto());

            return response;
        }

        public async Task<ServiceResponse<int>> AddGoodsAsync(int goodsId, decimal? price, int saleId, int userId)
        {
            var goods = await UnitOfWork.Get<Goods>().GetAsync(goodsId);
            if (goods == null || goods.SaleID.HasValue && goods.SaleID != saleId)
                return new ServiceResponse<int>(ServiceResponseStatus.NotFound);

            goods.SaleID = saleId;
            goods.Price = price;
            goods.UpdateModifiedFields(userId);

            UnitOfWork.Get<Goods>().Update(goods);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<int>(goodsId);
        }

        public async Task<ServiceResponse<IEnumerable<int>>> AddGoodsAsync(IEnumerable<int> goodsIds, int saleId, int userId)
        {
            var goods = await UnitOfWork.Get<Goods>().FindByAsync(g => goodsIds.Contains(g.ID) && !g.SaleID.HasValue);
            var goodsList = goods?.ToList();
            if (goodsList == null || !goodsList.Any())
                return new ServiceResponse<IEnumerable<int>>(ServiceResponseStatus.NotFound);

            goodsList.ForEach(g =>
            {
                g.SaleID = saleId;
                g.UpdateModifiedFields(userId);

                UnitOfWork.Get<Goods>().Update(g);
            });

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<IEnumerable<int>>(goodsList.Select(g => g.ID));
        }

        public async Task<ServiceResponse<int>> RemoveGoodsAsync(int goodsId, int saleId, int userId)
        {
            var goods = await UnitOfWork.Get<Goods>().GetAsync(goodsId);
            if (goods?.SaleID == null || goods.SaleID != saleId)
                return new ServiceResponse<int>(ServiceResponseStatus.NotFound);

            goods.SaleID = null;
            goods.Price = null;
            goods.UpdateModifiedFields(userId);

            UnitOfWork.Get<Goods>().Update(goods);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<int>(goodsId);
        }

        public async Task<ServiceResponse<IEnumerable<int>>> RemoveGoodsAsync(IEnumerable<int> goodsIds, int saleId, int userId)
        {
            var goods = await UnitOfWork.Get<Goods>().FindByAsync(g => goodsIds.Contains(g.ID) && g.SaleID.HasValue && g.SaleID.Value == saleId);
            var goodsList = goods?.ToList();
            if (goodsList == null || !goodsList.Any())
                return new ServiceResponse<IEnumerable<int>>(ServiceResponseStatus.NotFound);

            goodsList.ForEach(g =>
            {
                g.SaleID = null;
                g.Price = null;
                g.UpdateModifiedFields(userId);

                UnitOfWork.Get<Goods>().Update(g);
            });

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<IEnumerable<int>>(goodsList.Select(g => g.ID));
        }
    }
}
