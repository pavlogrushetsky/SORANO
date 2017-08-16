﻿using System;
using System.Linq;
using SORANO.BLL.Services.Abstract;
using SORANO.DAL.Repositories;
using System.Threading.Tasks;
using SORANO.BLL.Helpers;
using SORANO.CORE.StockEntities;

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
    }
}