using System;
using AutoMapper;

namespace SORANO.WEB.Mappings.Converters
{
    public class NullDateTimeToStringTypeConverter : ITypeConverter<DateTime?, string>
    {
        public string Convert(DateTime? source, string destination, ResolutionContext context)
        {
            return source?.ToString("dd.MM.yyyy") ?? string.Empty;
        }
    }
}