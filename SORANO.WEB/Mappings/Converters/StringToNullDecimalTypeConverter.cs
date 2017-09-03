using System.Globalization;
using AutoMapper;

namespace SORANO.WEB.Mappings.Converters
{
    public class StringToNullDecimalTypeConverter : ITypeConverter<string, decimal?>
    {
        public decimal? Convert(string source, decimal? destination, ResolutionContext context)
        {
            if (string.IsNullOrEmpty(source))
            {
                return null;
            }

            decimal.TryParse(source, NumberStyles.Any, new CultureInfo("en-US"), out var result);

            return result;
        }
    }
}