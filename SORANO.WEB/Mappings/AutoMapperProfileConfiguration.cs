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
using SORANO.WEB.ViewModels.Recommendation;

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

            CreateMap<AttachmentTypeDto, AttachmentTypeIndexViewModel>()
                .ForMember(
                    dest => dest.Extensions,
                    source => source.MapFrom(s => s.Extensions.Split(','))
                );

            CreateMap<ArticleTypeDto, ArticleTypeIndexViewModel>();
            CreateMap<ArticleTypeCreateUpdateViewModel, ArticleTypeDto>();
            CreateMap<ArticleTypeDto, ArticleTypeDetailsViewModel>();
            CreateMap<ArticleTypeDto, ArticleTypeDeleteViewModel>();

            CreateMap<ArticleDto, ArticleIndexViewModel>()
                .ForMember(
                    dest => dest.TypeName, 
                    source => source.MapFrom(s => s.Type.Name)
                );
            CreateMap<ArticleDto, ArticleCreateUpdateViewModel>();
            CreateMap<ArticleDto, ArticleDetailsViewModel>();
            CreateMap<ArticleDto, ArticleDeleteViewModel>();
        }
    }
}