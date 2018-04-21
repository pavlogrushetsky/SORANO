using System.Globalization;
using AutoMapper;

namespace SORANO.WEB.Mappings.Converters
{
    public class NullDecimalToStringTypeConverter : ITypeConverter<decimal?, string>
    {
        public string Convert(decimal? source, string destination, ResolutionContext context)
        {
            return source?.ToString("0.00", new CultureInfo("ru-RU")) ?? "0,00";
        }
    }
}