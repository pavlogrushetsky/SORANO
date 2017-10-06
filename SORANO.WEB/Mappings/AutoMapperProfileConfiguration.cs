using System;
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
using SORANO.WEB.ViewModels.Location;
using SORANO.WEB.ViewModels.LocationType;
using SORANO.WEB.ViewModels.Recommendation;
using SORANO.WEB.ViewModels.Supplier;

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

            CreateMap<UserDto, AccountViewModel>();

            CreateMap<RecommendationDto, RecommendationViewModel>();
            CreateMap<RecommendationViewModel, RecommendationDto>();

            CreateMap<AttachmentDto, AttachmentViewModel>()
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
            CreateMap<AttachmentViewModel, AttachmentDto>();

            CreateMap<MainPictureViewModel, AttachmentDto>();
            CreateMap<AttachmentDto, MainPictureViewModel>();

            CreateMap<AttachmentTypeDto, AttachmentTypeIndexViewModel>()
                .ForMember(
                    dest => dest.Extensions,
                    source => source.MapFrom(s => s.Extensions.Split(','))
                );

            CreateMap<ArticleTypeDto, ArticleTypeIndexViewModel>()
                .ForMember(
                    dest => dest.MainPicturePath,
                    source => source.MapFrom(s => s.MainPicture.FullPath)
                );
            CreateMap<ArticleTypeCreateUpdateViewModel, ArticleTypeDto>();
            CreateMap<ArticleTypeDto, ArticleTypeCreateUpdateViewModel>();

            CreateMap<ArticleTypeDto, ArticleTypeDetailsViewModel>();
            CreateMap<ArticleTypeDto, ArticleTypeDeleteViewModel>();

            #region AttachmentType

            CreateMap<AttachmentTypeDto, AttachmentTypeIndexViewModel>()
                .ForMember(
                    dest => dest.Extensions,
                    source => source.MapFrom(s => s.Extensions.Split(','))
                );
            CreateMap<AttachmentTypeDto, AttachmentTypeCreateUpdateViewModel>();
            CreateMap<AttachmentTypeDto, AttachmentTypeDeleteViewModel>();
            CreateMap<AttachmentTypeCreateUpdateViewModel, AttachmentTypeDto>();

            #endregion

            #region Location

            CreateMap<LocationDto, LocationIndexViewModel>();
            CreateMap<LocationDto, LocationCreateUpdateViewModel>();
            CreateMap<LocationDto, LocationDetailsViewModel>();
            CreateMap<LocationDto, LocationDeleteViewModel>();
            CreateMap<LocationCreateUpdateViewModel, LocationDto>();

            #endregion

            #region LocationType

            CreateMap<LocationTypeDto, LocationTypeIndexViewModel>();
            CreateMap<LocationTypeDto, LocationTypeCreateUpdateViewModel>();
            CreateMap<LocationTypeDto, LocationTypeDetailsViewModel>();
            CreateMap<LocationTypeDto, LocationTypeDeleteViewModel>();
            CreateMap<LocationTypeCreateUpdateViewModel, LocationTypeDto>();

            #endregion

            #region Client

            CreateMap<ClientDto, ClientIndexViewModel>();
            CreateMap<ClientDto, ClientCreateUpdateViewModel>();
            CreateMap<ClientDto, ClientDetailsViewModel>();
            CreateMap<ClientDto, ClientDeleteViewModel>();
            CreateMap<ClientCreateUpdateViewModel, ClientDto>();

            #endregion

            #region Supplier

            CreateMap<SupplierDto, SupplierIndexViewModel>();
            CreateMap<SupplierDto, SupplierCreateUpdateViewModel>();
            CreateMap<SupplierDto, SupplierDetailsViewModel>();
            CreateMap<SupplierDto, SupplierDeleteViewModel>();
            CreateMap<SupplierCreateUpdateViewModel, SupplierDto>();

            #endregion

            #region Article

            CreateMap<ArticleDto, ArticleIndexViewModel>();
            CreateMap<ArticleDto, ArticleCreateUpdateViewModel>();            
            CreateMap<ArticleDto, ArticleDetailsViewModel>();
            CreateMap<ArticleDto, ArticleDeleteViewModel>();
            CreateMap<ArticleCreateUpdateViewModel, ArticleDto>();

            #endregion
        }
    }
}