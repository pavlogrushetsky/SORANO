﻿using System;
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
            if (await IsAccessDenied(userId))
                return new AccessDeniedResponse<int>();

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

        public async Task<ServiceResponse<IEnumerable<AllGoodsDto>>> GetAllAsync(int userId)
        {
            if (await IsAccessDenied(userId))
                return new AccessDeniedResponse<IEnumerable<AllGoodsDto>>();

            var goods = await UnitOfWork.Get<Goods>().GetAllAsync();

            var result = goods
                .Where(g => !g.SaleDate.HasValue)
                .GroupBy(g => new
                {
                    g.DeliveryItem.Article,
                    g.Storages.Single(s => !s.ToDate.HasValue).Location,
                    g.DeliveryItem.Delivery,
                    g.DeliveryItem.UnitPrice,
                    g.DeliveryItem.Delivery.DollarRate,
                    g.DeliveryItem.Delivery.EuroRate
                })
                .GroupBy(g => g.Key.Article)
                .Select(g => new AllGoodsDto
                {
                    ArticleId = g.Key.ID,
                    ArticleName = g.Key.Name,
                    ArticleImage = g.Key.Attachments.FirstOrDefault(a => a.Type.Name.Equals("Основное изображение"))
                        ?.FullPath,
                    Goods = g.Select(gr => new GoodsGroupDto
                        {
                            BillNumber = gr.Key.Delivery.BillNumber,
                            Count = gr.Count(),
                            DeliveryId = gr.Key.Delivery.ID,
                            DeliveryPrice = gr.Key.UnitPrice,
                            DollarRate = gr.Key.DollarRate,
                            EuroRate = gr.Key.EuroRate,
                            LocationId = gr.Key.Location.ID,
                            LocationName = gr.Key.Location.Name
                        })
                        .ToList()
                })
                .ToList();

            return new SuccessResponse<IEnumerable<AllGoodsDto>>(result);
        }

        public async Task<ServiceResponse<IEnumerable<GoodsDto>>> GetSoldGoodsAsync(int userId)
        {
            if (await IsAccessDenied(userId))
                return new AccessDeniedResponse<IEnumerable<GoodsDto>>();

            var goods = await UnitOfWork.Get<Goods>().FindByAsync(g => g.SaleDate.HasValue && g.SaleLocationID.HasValue && g.SalePrice.HasValue);

            return new SuccessResponse<IEnumerable<GoodsDto>>(goods.Select(g => g.ToDto()));
        }

        public async Task<ServiceResponse<decimal>> GetTotalIncomeAsync(int userId)
        {
            if (await IsAccessDenied(userId))
                return new AccessDeniedResponse<decimal>();

            var goods = await UnitOfWork.Get<Goods>().FindByAsync(g => g.SaleDate.HasValue && g.SaleLocationID.HasValue && g.SalePrice.HasValue);

            var sum = goods.Sum(g => g.SalePrice.Value);

            return new SuccessResponse<decimal>(sum);
        }

        public async Task<ServiceResponse<int>> SaleAsync(int articleId, int locationId, int clientId, int num, decimal price, int userId)
        {
            if (await IsAccessDenied(userId))
                return new AccessDeniedResponse<int>();

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
