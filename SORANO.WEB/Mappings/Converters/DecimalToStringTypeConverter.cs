using AutoMapper;

namespace SORANO.WEB.Mappings.Converters
{
    public class DecimalToStringTypeConverter : ITypeConverter<decimal, string>
    {
        public string Convert(decimal source, string destination, ResolutionContext context)
        {
            return source.ToString("0.00");
        }
    }
}