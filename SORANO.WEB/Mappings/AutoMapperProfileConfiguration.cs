using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AutoMapper;
using SORANO.BLL.Dtos;
using SORANO.WEB.Mappings.Converters;
using SORANO.WEB.ViewModels.Account;
using SORANO.WEB.ViewModels.Article;
using SORANO.WEB.ViewModels.ArticleType;
using SORANO.WEB.ViewModels.Attachment;
using SORANO.WEB.ViewModels.AttachmentType;
using SORANO.WEB.ViewModels.Client;
using SORANO.WEB.ViewModels.Delivery;
using SORANO.WEB.ViewModels.DeliveryItem;
using SORANO.WEB.ViewModels.Goods;
using SORANO.WEB.ViewModels.Location;
using SORANO.WEB.ViewModels.LocationType;
using SORANO.WEB.ViewModels.Recommendation;
using SORANO.WEB.ViewModels.Sale;
using SORANO.WEB.ViewModels.Storage;
using SORANO.WEB.ViewModels.Supplier;
using SORANO.WEB.ViewModels.User;

namespace SORANO.WEB.Mappings
{
    public class AutoMapperProfileConfiguration : Profile
    {
        public AutoMapperProfileConfiguration() : this("SORANO")
        {           
        }

        protected AutoMapperProfileConfiguration(string profileName) : base(profileName)
        {
            CreateMap<int, int?>().ConvertUsing<IntToNullIntTypeConverter>();
            CreateMap<string, decimal?>().ConvertUsing<StringToNullDecimalTypeConverter>();
            CreateMap<decimal?, string>().ConvertUsing<NullDecimalToStringTypeConverter>();
            CreateMap<string, decimal>().ConvertUsing<StringToDecimalTypeConverter>();
            CreateMap<decimal, string>().ConvertUsing<DecimalToStringTypeConverter>();
            CreateMap<DateTime?, string>().ConvertUsing<NullDateTimeToStringTypeConverter>();
            CreateMap<DateTime, string>().ConvertUsing<DateTimeToStringTypeConverter>();
            CreateMap<string, DateTime>().ConvertUsing<StringToDateTimeTypeConverter>();
            CreateMap<string, DateTime?>().ConvertUsing<StringToNullDateTimeTypeConverter>();

            CreateMap<UserDto, AccountViewModel>();

            #region Recommendation

            CreateMap<RecommendationDto, RecommendationViewModel>();
            CreateMap<RecommendationViewModel, RecommendationDto>();

            #endregion

            #region Attachment

            CreateMap<AttachmentDto, AttachmentViewModel>()
                .ForMember(
                    dest => dest.TypeID,
                    source => source.MapFrom(s => s.AttachmentType.ID)
                )
                .ForMember(
                    dest => dest.TypeName,
                    source => source.MapFrom(s => s.AttachmentType.Name)
                )
                .ForMember(
                    dest => dest.MimeTypes,
                    source => source.MapFrom(s => !string.IsNullOrEmpty(s.AttachmentType.Extensions)
                        ? string.Join(",", s.AttachmentType.Extensions.Split(',').Select(MimeTypes.MimeTypeMap.GetMimeType))
                        : "")
                );
            CreateMap<AttachmentViewModel, AttachmentDto>()
                .ForMember(
                    dest => dest.AttachmentTypeID,
                    source => source.MapFrom(s => s.TypeID)
                );
           
            CreateMap<AttachmentDto, MainPictureViewModel>()
                .ForMember(
                    dest => dest.TypeID,
                    source => source.MapFrom(s => s.AttachmentType.ID)
                );
            CreateMap<MainPictureViewModel, AttachmentDto>()
                .ForMember(
                    dest => dest.AttachmentTypeID,
                    source => source.MapFrom(s => s.TypeID)
                );

            #endregion

            #region Storage

            CreateMap<StorageDto, StorageViewModel>()
                .ForMember(
                    dest => dest.LocationName,
                    source => source.MapFrom(s => s.Location.Name)
                )
                .ForMember(
                    dest => dest.LocationDescription,
                    source => source.MapFrom(s => s.Location.Comment)
                );

            #endregion

            #region AttachmentType

            CreateMap<AttachmentTypeDto, AttachmentTypeIndexViewModel>()
                .ForMember(
                    dest => dest.Extensions,
                    source => source.MapFrom(s => s.Extensions.Split(','))
                )
                .ForMember(
                    dest => dest.MimeTypes,
                    source => source.MapFrom(s => !string.IsNullOrEmpty(s.Extensions)
                        ? string.Join(",", s.Extensions.Split(',').Select(MimeTypes.MimeTypeMap.GetMimeType))
                        : "")
                );
            CreateMap<AttachmentTypeDto, AttachmentTypeCreateUpdateViewModel>();
            CreateMap<AttachmentTypeDto, AttachmentTypeDeleteViewModel>();
            CreateMap<AttachmentTypeCreateUpdateViewModel, AttachmentTypeDto>();

            #endregion

            #region Location

            CreateMap<LocationDto, LocationIndexViewModel>();
            CreateMap<LocationDto, LocationCreateUpdateViewModel>();
            CreateMap<LocationDto, LocationDetailsViewModel>()
                .ForMember(
                    dest => dest.MainPicturePath,
                    source => source.MapFrom(s => s.MainPicture.FullPath)
                );
            CreateMap<LocationDto, LocationDeleteViewModel>();
            CreateMap<LocationCreateUpdateViewModel, LocationDto>();

            #endregion

            #region LocationType

            CreateMap<LocationTypeDto, LocationTypeIndexViewModel>();
            CreateMap<LocationTypeDto, LocationTypeCreateUpdateViewModel>();
            CreateMap<LocationTypeDto, LocationTypeDetailsViewModel>()
                .ForMember(
                    dest => dest.MainPicturePath,
                    source => source.MapFrom(s => s.MainPicture.FullPath)
                );
            CreateMap<LocationTypeDto, LocationTypeDeleteViewModel>();
            CreateMap<LocationTypeCreateUpdateViewModel, LocationTypeDto>();

            #endregion

            #region Client

            CreateMap<ClientDto, ClientIndexViewModel>();
            CreateMap<ClientDto, ClientCreateUpdateViewModel>();
            CreateMap<ClientDto, ClientDetailsViewModel>()
                .ForMember(
                    dest => dest.MainPicturePath,
                    source => source.MapFrom(s => s.MainPicture.FullPath)
                );
            CreateMap<ClientDto, ClientDeleteViewModel>();
            CreateMap<ClientCreateUpdateViewModel, ClientDto>();

            #endregion

            #region Supplier

            CreateMap<SupplierDto, SupplierIndexViewModel>();
            CreateMap<SupplierDto, SupplierCreateUpdateViewModel>();
            CreateMap<SupplierDto, SupplierDetailsViewModel>()
                .ForMember(
                    dest => dest.MainPicturePath,
                    source => source.MapFrom(s => s.MainPicture.FullPath)
                );
            CreateMap<SupplierDto, SupplierDeleteViewModel>();
            CreateMap<SupplierCreateUpdateViewModel, SupplierDto>();

            #endregion

            #region Article

            CreateMap<ArticleDto, ArticleViewModel>();
            CreateMap<IEnumerable<ArticleDto>, ArticleTableViewModel>()
                .ForMember(
                    dest => dest.Articles,
                    source => source.MapFrom(s => s)
                );
            CreateMap<ArticleDto, ArticleSelectViewModel>()
                .ForMember(
                    dest => dest.Type,
                    source => source.MapFrom(s => $"Тип: {s.Type.Name}")
                )
                .ForMember(
                    dest => dest.Code,
                    source => source.MapFrom(s => !string.IsNullOrWhiteSpace(s.Code) ? $"Код: {s.Code}" : string.Empty)
                )
                .ForMember(
                    dest => dest.Barcode,
                    source => source.MapFrom(s => !string.IsNullOrWhiteSpace(s.Barcode) ? $"Штрих-код: {s.Barcode}" : string.Empty)
                );
            CreateMap<ArticleDto, ArticleCreateUpdateViewModel>();            
            CreateMap<ArticleDto, ArticleDetailsViewModel>()
                .ForMember(
                    dest => dest.MainPicturePath,
                    source => source.MapFrom(s => s.MainPicture.FullPath)
                )
                .ForMember(
                    dest => dest.Table,
                    source => source.MapFrom(s => s.DeliveryItems)
                );
            CreateMap<ArticleDto, ArticleDeleteViewModel>();
            CreateMap<ArticleCreateUpdateViewModel, ArticleDto>();
            CreateMap<ArticleDto, ArticleViewModel>()
                .ForMember(
                    dest => dest.MainPicturePath,
                    source => source.MapFrom(s => s.MainPicture.FullPath)
                )
                .ForMember(
                    dest => dest.TypeName,
                    source => source.MapFrom(s => s.Type.Type == null ? s.Type.Name : $"{s.Type.Type.Name}  {'\u21d0'}  {s.Type.Name}")
                );

            #endregion

            #region ArticleType

            CreateMap<ArticleTypeDto, ArticleTypeIndexViewModel>()
                .ForMember(
                    dest => dest.MainPicturePath,
                    source => source.MapFrom(s => s.MainPicture.FullPath)
                );
            CreateMap<IEnumerable<ArticleTypeDto>, ArticleTypeTreeViewModel>()
                .ForMember(
                    dest => dest.ArticleTypes,
                    source => source.MapFrom(s => s)
                );
            CreateMap<ArticleTypeCreateUpdateViewModel, ArticleTypeDto>();
            CreateMap<ArticleTypeDto, ArticleTypeCreateUpdateViewModel>();
            CreateMap<ArticleTypeDto, ArticleTypeDetailsViewModel>()
                .ForMember(
                    dest => dest.MainPicturePath,
                    source => source.MapFrom(s => s.MainPicture.FullPath)
                );
            CreateMap<ArticleTypeDto, ArticleTypeDeleteViewModel>();

            #endregion

            #region Delivery

            CreateMap<DeliveryDto, DeliveryViewModel>()
                .ForMember(
                    dest => dest.DeliveryItemsCount,
                    source => source.MapFrom(s => s.Items.Count())
                )
                .ForMember(
                    dest => dest.SupplierName,
                    source => source.MapFrom(s => s.Supplier.Name)
                )
                .ForMember(
                    dest => dest.LocationName,
                    source => source.MapFrom(s => s.Location.Name)
                )
                .ForMember(
                    dest => dest.TotalPrice,
                    source => source.MapFrom(s => s.TotalDiscountedPrice)
                )
                .ForMember(
                    dest => dest.Currency,
                    source => source.MapFrom(s => s.DollarRate.HasValue ? "$" : s.EuroRate.HasValue ? "€" : "₴")
                )
                .ForMember(
                    dest => dest.CanBeUpdated,
                    source => source.MapFrom(s => s.CanBeDeleted)
                );
            CreateMap<IEnumerable<DeliveryDto>, DeliveryIndexViewModel>()
                .ForMember(
                    dest => dest.Items,
                    source => source.MapFrom(s => s)
                );

            CreateMap<DeliveryDto, DeliveryCreateUpdateViewModel>()
                .ForMember(
                    dest => dest.ItemsCount,
                    source => source.MapFrom(s => s.Items.Count())
                )
                .ForMember(
                    dest => dest.SelectedCurrency,
                    source => source.MapFrom(s => s.DollarRate.HasValue ? "$" : s.EuroRate.HasValue ? "€" : "₴")
                );
            CreateMap<DeliveryCreateUpdateViewModel, DeliveryDto>();
            CreateMap<DeliveryDto, DeliveryDetailsViewModel>()
                .ForMember(
                    dest => dest.MainPicturePath,
                    source => source.MapFrom(s => s.MainPicture.FullPath)
                )
                .ForMember(
                    dest => dest.Currency,
                    source => source.MapFrom(s => s.DollarRate.HasValue ? "$" : s.EuroRate.HasValue ? "€" : "₴")
                )
                .ForMember(
                    dest => dest.SupplierName,
                    source => source.MapFrom(s => s.Supplier.Name)
                )
                .ForMember(
                    dest => dest.LocationName,
                    source => source.MapFrom(s => s.Location.Name)
                )
                .ForMember(
                    dest => dest.Table,
                    source => source.MapFrom(s => s.Items)
                );
            CreateMap<DeliveryDto, DeliveryDeleteViewModel>()
                .ForMember(
                    dest => dest.SupplierName,
                    source => source.MapFrom(s => s.Supplier.Name)
                )
                .ForMember(
                    dest => dest.LocationName,
                    source => source.MapFrom(s => s.Location.Name)
                );

            CreateMap<DeliveryItemDto, DeliveryItemViewModel>()
                .ForMember(
                    dest => dest.Currency,
                    source => source.MapFrom(s => s.Delivery.DollarRate.HasValue ? "$" : s.Delivery.EuroRate.HasValue ? "€" : "₴")
                )
                .ForMember(
                    dest => dest.DeliveryBillNumber,
                    source => source.MapFrom(s => s.Delivery.BillNumber)
                )
                .ForMember(
                    dest => dest.DeliveryDate,
                    source => source.MapFrom(s => s.Delivery.DeliveryDate)
                );
            CreateMap<IEnumerable<DeliveryItemDto>, DeliveryItemTableViewModel>()
                .ForMember(
                    dest => dest.Items,
                    source => source.MapFrom(s => s)
                );
            CreateMap<DeliveryItemsSummaryDto, DeliveryItemsSummaryViewModel>();
            CreateMap<DeliveryItemViewModel, DeliveryItemDto>();

            #endregion

            #region Sale

            CreateMap<SaleDto, SaleViewModel>()
                .ForMember(
                    dest => dest.SaleItemsCount,
                    source => source.MapFrom(s => s.Goods.Count())
                )
                .ForMember(
                    dest => dest.LocationName,
                    source => source.MapFrom(s => s.Location.Name)
                )
                .ForMember(
                    dest => dest.UserName,
                    source => source.MapFrom(s => s.User.Login)
                )
                .ForMember(
                    dest => dest.ClientName,
                    source => source.MapFrom(s => s.Client == null ? string.Empty : s.Client.Name)
                )
                .ForMember(
                    dest => dest.Currency,
                    source => source.MapFrom(s => s.DollarRate.HasValue ? "$" : s.EuroRate.HasValue ? "€" : "₴")
                )
                .ForMember(
                    dest => dest.CanBeUpdated,
                    source => source.MapFrom(s => s.CanBeDeleted)
                );
            CreateMap<IEnumerable<SaleDto>, SaleIndexViewModel>()
                .ForMember(
                    dest => dest.Items,
                    source => source.MapFrom(s => s)
                );
            CreateMap<SaleCreateUpdateViewModel, SaleDto>();
            CreateMap<SaleDto, SaleCreateUpdateViewModel>()
                .ForMember(
                    dest => dest.SelectedCurrency,
                    source => source.MapFrom(s => s.DollarRate.HasValue ? "$" : s.EuroRate.HasValue ? "€" : "₴")
                );
            CreateMap<SaleDto, SaleDeleteViewModel>();
            CreateMap<SaleDto, SaleDetailsViewModel>()
                .ForMember(
                    dest => dest.Currency,
                    source => source.MapFrom(s => s.DollarRate.HasValue ? "$" : s.EuroRate.HasValue ? "€" : "₴")
                )
                .ForMember(
                    dest => dest.ClientName,
                    source => source.MapFrom(s => s.Client == null ? string.Empty : s.Client.Name)
                )
                .ForMember(
                    dest => dest.LocationName,
                    source => source.MapFrom(s => s.Location.Name)
                )
                .ForMember(
                    dest => dest.UserName,
                    source => source.MapFrom(s => s.User.Login)
                );
            CreateMap<SaleItemsSummaryDto, SaleItemsSummaryViewModel>()
                .ForMember(
                    dest => dest.SelectedCount,
                    source => source.MapFrom(s => s.Count)
                )
                .ForMember(
                    dest => dest.TotalPrice,
                    source => source.MapFrom(s => s.TotalPrice.HasValue ? s.TotalPrice.Value.ToString("0.00", new CultureInfo("ru-RU")) : "0,00")
                )
                .ForMember(
                    dest => dest.SelectedCurrency,
                    source => source.MapFrom(s => s.Currency == Currency.Hryvna ? "₴" : s.Currency == Currency.Euro ? "€" : "$")
                );

            CreateMap<SaleItemsGroupsDto, SaleItemsGroupsViewModel>();
            CreateMap<SaleItemsGroupDto, SaleItemsGroupViewModel>()
                .ForMember(
                    dest => dest.HasMainPicture,
                    source => source.MapFrom(s => !string.IsNullOrWhiteSpace(s.MainPicturePath))
                )
                .ForMember(
                    dest => dest.IsSelected,
                    source => source.MapFrom(s => s.Items.All(i => i.IsSelected))
                );
            CreateMap<SaleItemDto, SaleItemViewModel>();
            CreateMap<RecommendationDto, SaleItemRecommendationViewModel>();

            #endregion

            #region Goods

            CreateMap<GoodsIndexViewModel, GoodsFilterCriteriaDto>();

            CreateMap<GoodsDto, GoodsItemViewModel>()
                .ForMember(
                    dest => dest.GoodsIds,
                    source => source.MapFrom(s => s.IDs.Select(id => id.ToString()).Aggregate((i, j) => i + ',' + j))
                )
                .ForMember(
                    dest => dest.ArticleID,
                    source => source.MapFrom(s => s.DeliveryItem.ArticleID)
                )
                .ForMember(
                    dest => dest.ArticleName,
                    source => source.MapFrom(s => s.DeliveryItem.Article.Name)
                )
                .ForMember(
                    dest => dest.ArticleDescription,
                    source => source.MapFrom(s => s.DeliveryItem.Article.Description)
                )
                .ForMember(
                    dest => dest.ArticleTypeID,
                    source => source.MapFrom(s => s.DeliveryItem.Article.TypeID)
                )
                .ForMember(
                    dest => dest.ArticleTypeName,
                    source => source.MapFrom(s => s.DeliveryItem.Article.Type.Name)
                )
                .ForMember(
                    dest => dest.LocationID,
                    source => source.MapFrom(s => s.Storages.OrderBy(st => st.FromDate).Last().LocationID)
                )
                .ForMember(
                    dest => dest.LocationName,
                    source => source.MapFrom(s => s.Storages.OrderBy(st => st.FromDate).Last().Location.Name)
                )
                .ForMember(
                    dest => dest.IsSold,
                    source => source.MapFrom(s => s.SaleID.HasValue)
                )                
                .ForMember(
                    dest => dest.ImagePath,
                    source => source.MapFrom(s => s.DeliveryItem.Article.MainPicture.FullPath)
                );

            CreateMap<GoodsDto, GoodsDetailsViewModel>()
                .ForMember(
                    dest => dest.ArticleID,
                    source => source.MapFrom(s => s.DeliveryItem.ArticleID)
                )
                .ForMember(
                    dest => dest.ArticleName,
                    source => source.MapFrom(s => s.DeliveryItem.Article.Name)
                )
                .ForMember(
                    dest => dest.ArticleDescription,
                    source => source.MapFrom(s => s.DeliveryItem.Article.Description)
                )
                .ForMember(
                    dest => dest.ArticleTypeID,
                    source => source.MapFrom(s => s.DeliveryItem.Article.TypeID)
                )
                .ForMember(
                    dest => dest.ArticleTypeName,
                    source => source.MapFrom(s => s.DeliveryItem.Article.Type.Name)
                )
                .ForMember(
                    dest => dest.DeliveryID,
                    source => source.MapFrom(s => s.DeliveryItem.DeliveryID)
                )
                .ForMember(
                    dest => dest.DeliveryBillNumber,
                    source => source.MapFrom(s => s.DeliveryItem.Delivery.BillNumber)
                )
                .ForMember(
                    dest => dest.DeliveryPrice,
                    source => source.MapFrom(s => s.DeliveryItem.UnitPrice)
                )
                .ForMember(
                    dest => dest.DeliveryDate,
                    source => source.MapFrom(s => s.DeliveryItem.Delivery.DeliveryDate)
                )
                .ForMember(
                    dest => dest.Currency,
                    source => source.MapFrom(s => s.DeliveryItem.Delivery.DollarRate.HasValue ? "$" : s.DeliveryItem.Delivery.EuroRate.HasValue ? "€" : "₴")
                )
                .ForMember(
                    dest => dest.LocationID,
                    source => source.MapFrom(s => s.Storages.OrderBy(st => st.FromDate).Last().LocationID)
                )
                .ForMember(
                    dest => dest.LocationName,
                    source => source.MapFrom(s => s.Storages.OrderBy(st => st.FromDate).Last().Location.Name)
                )
                .ForMember(
                    dest => dest.IsSold,
                    source => source.MapFrom(s => s.SaleID.HasValue)
                )
                .ForMember(
                    dest => dest.SoldBy,
                    source => source.MapFrom(s => s.SoldByUser)
                )
                .ForMember(
                    dest => dest.MainPicturePath,
                    source => source.MapFrom(s => s.DeliveryItem.Article.MainPicture.FullPath)
                )
                .ForMember(
                    dest => dest.DeliveryRecommendations,
                    source => source.MapFrom(s => s.DeliveryItem.Delivery.Recommendations)
                )
                .ForMember(
                    dest => dest.DeliveryItemRecommendations,
                    source => source.MapFrom(s => s.DeliveryItem.Recommendations)
                );

            CreateMap<GoodsDto, GoodsRecommendationsViewModel>()
                .ForMember(
                    dest => dest.ArticleName,
                    source => source.MapFrom(s => s.DeliveryItem.Article.Name)
                )
                .ForMember(
                    dest => dest.ArticleDescription,
                    source => source.MapFrom(s => s.DeliveryItem.Article.Description)
                )
                .ForMember(
                    dest => dest.ArticleTypeName,
                    source => source.MapFrom(s => s.DeliveryItem.Article.Type.Name)
                )
                .ForMember(
                    dest => dest.DeliveryPrice,
                    source => source.MapFrom(s => s.DeliveryItem.UnitPrice)
                )
                .ForMember(
                    dest => dest.Currency,
                    source => source.MapFrom(s => s.DeliveryItem.Delivery.DollarRate.HasValue ? "$" : s.DeliveryItem.Delivery.EuroRate.HasValue ? "€" : "₴")
                )
                .ForMember(
                    dest => dest.LocationName,
                    source => source.MapFrom(s => s.Storages.OrderBy(st => st.FromDate).Last().Location.Name)
                )
                .ForMember(
                    dest => dest.MainPicture,
                    source => source.MapFrom(s => new MainPictureViewModel{ FullPath = s.DeliveryItem.Article.MainPicture.FullPath })
                );

            CreateMap<GoodsDto, GoodsChangeLocationViewModel>()
                .ForMember(
                    dest => dest.ArticleName,
                    source => source.MapFrom(s => s.DeliveryItem.Article.Name)
                )
                .ForMember(
                    dest => dest.ArticleDescription,
                    source => source.MapFrom(s => s.DeliveryItem.Article.Description)
                )
                .ForMember(
                    dest => dest.ArticleTypeName,
                    source => source.MapFrom(s => s.DeliveryItem.Article.Type.Name)
                )
                .ForMember(
                    dest => dest.CurrentLocationName,
                    source => source.MapFrom(s => s.Storages.OrderBy(st => st.FromDate).Last().Location.Name)
                )
                .ForMember(
                    dest => dest.CurrentLocationID,
                    source => source.MapFrom(s => s.Storages.OrderBy(st => st.FromDate).Last().Location.ID)
                )
                .ForMember(
                    dest => dest.MainPicture,
                    source => source.MapFrom(s => new MainPictureViewModel { FullPath = s.DeliveryItem.Article.MainPicture.FullPath })
                );

            #endregion

            #region User

            CreateMap<UserDto, UserIndexViewModel>()
                .ForMember(
                    dest => dest.Roles,
                    source => source.MapFrom(s => s.Roles.Select(r => r.Description))
                )
                .ForMember(
                    dest => dest.Locations,
                    source => source.MapFrom(s => s.Locations.Select(r => r.Name))
                );
            CreateMap<UserActivityDto, UserActivityViewModel>();
            CreateMap<UserActivityType, string>().ConvertUsing<UserActivityTypeToStringConverter>();
            CreateMap<UserDto, UserCreateUpdateViewModel>()
                .ForMember(
                    dest => dest.RoleIDs,
                    source => source.MapFrom(s => s.Roles.Select(r => r.ID))
                )
                .ForMember(
                    dest => dest.Roles,
                    source => source.Ignore()
                )
                .ForMember(
                    dest => dest.LocationIds,
                    source => source.MapFrom(s => s.Locations.Select(r => r.ID))
                )
                .ForMember(
                    dest => dest.LocationNames,
                    source => source.Ignore()
                );
            CreateMap<UserDto, UserDetailsViewModel>()
                .ForMember(
                    dest => dest.Roles,
                    source => source.MapFrom(s => s.Roles.Select(r => r.Description))
                )
                .ForMember(
                    dest => dest.Locations,
                    source => source.MapFrom(s => s.Locations.Select(r => r.Name))
                );
            CreateMap<UserDto, UserBlockViewModel>();
            CreateMap<UserDto, UserDeleteViewModel>();
            CreateMap<UserCreateUpdateViewModel, UserDto>()
                .ForMember(
                    dest => dest.Password,
                    source => source.MapFrom(s => s.NewPassword ?? s.Password)
                )
                .ForMember(
                    dest => dest.Roles,
                    source => source.MapFrom(s => s.RoleIDs.Select(r => new RoleDto
                    {
                        ID = r
                    }))
                )
                .ForMember(
                    dest => dest.Locations,
                    source => source.MapFrom(s => s.LocationIds.Select(r => new LocationDto
                    {
                        ID = r
                    }))
                );

            #endregion
        }
    }
}