using AutoMapper;

namespace SORANO.WEB.Mappings.Converters
{
    public class NullDecimalToStringTypeConverter : ITypeConverter<decimal?, string>
    {
        public string Convert(decimal? source, string destination, ResolutionContext context)
        {
            return !source.HasValue ? string.Empty : source.Value.ToString("0.00");
        }
    }
}