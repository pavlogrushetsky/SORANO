using System;
using AutoMapper;
using SORANO.BLL.DTOs;
using SORANO.WEB.Mappings.Converters;
using SORANO.WEB.ViewModels;
using SORANO.WEB.ViewModels.Article;
using SORANO.WEB.ViewModels.ArticleType;

namespace SORANO.WEB.Mappings
{
    public class AutoMapperProfileConfiguration : Profile
    {
        public AutoMapperProfileConfiguration() : this("SORANO")
        {           
        }

        protected AutoMapperProfileConfiguration(string profileName) : base(profileName)
        {
            CreateMap<string, decimal?>().ConvertUsing<StringToNullDecimalTypeConverter>();
            CreateMap<decimal?, string>().ConvertUsing<NullDecimalToStringTypeConverter>();
            CreateMap<string, decimal>().ConvertUsing<StringToDecimalTypeConverter>();
            CreateMap<decimal, string>().ConvertUsing<DecimalToStringTypeConverter>();
            CreateMap<DateTime?, string>().ConvertUsing<NullDateTimeToStringTypeConverter>();
            CreateMap<DateTime, string>().ConvertUsing<DateTimeToStringTypeConverter>();

            CreateMap<RecommendationDto, RecommendationViewModel>();
            CreateMap<RecommendationViewModel, RecommendationDto>();

            CreateMap<AttachmentDto, AttachmentViewModel>();
            CreateMap<AttachmentViewModel, AttachmentDto>();

            CreateMap<ArticleTypeDto, ArticleTypeIndexViewModel>();
            CreateMap<ArticleTypeDto, ArticleTypeCreateUpdateViewModel>();
            CreateMap<ArticleTypeDto, ArticleTypeDetailsViewModel>();
            CreateMap<ArticleTypeDto, ArticleTypeDeleteViewModel>();

            CreateMap<ArticleDto, ArticleIndexViewModel>()
                .ForMember(dest => dest.TypeName, source => source.MapFrom(s => s.Type.Name));
            CreateMap<ArticleDto, ArticleCreateUpdateViewModel>();
            CreateMap<ArticleDto, ArticleDetailsViewModel>();
            CreateMap<ArticleDto, ArticleDeleteViewModel>();
        }
    }
}