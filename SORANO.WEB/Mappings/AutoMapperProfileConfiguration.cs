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

            CreateMap<ArticleDto, ArticleIndexViewModel>();
            CreateMap<ArticleDto, ArticleCreateUpdateViewModel>();            
            CreateMap<ArticleDto, ArticleDetailsViewModel>()
                .ForMember(
                    dest => dest.MainPicturePath,
                    source => source.MapFrom(s => s.MainPicture.FullPath)
                );
            CreateMap<ArticleDto, ArticleDeleteViewModel>();
            CreateMap<ArticleCreateUpdateViewModel, ArticleDto>();

            #endregion

            #region ArticleType

            CreateMap<ArticleTypeDto, ArticleTypeIndexViewModel>()
                .ForMember(
                    dest => dest.MainPicturePath,
                    source => source.MapFrom(s => s.MainPicture.FullPath)
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
        }
    }
}