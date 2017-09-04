using System;
using AutoMapper;
using SORANO.BLL.DTOs;
using SORANO.WEB.Mappings.Converters;
using SORANO.WEB.ViewModels;

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

            CreateMap<ArticleDto, ArticleViewModel>();
            CreateMap<ArticleViewModel, ArticleDto>();

            CreateMap<DetailsDto, DetailsViewModel>();

            CreateMap<ArticleDetailedDto, ArticleDetailedViewModel>();
        }
    }
}