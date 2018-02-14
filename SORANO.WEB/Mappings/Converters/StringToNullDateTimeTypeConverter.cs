using System;
using System.Globalization;
using AutoMapper;

namespace SORANO.WEB.Mappings.Converters
{
    public class StringToNullDateTimeTypeConverter : ITypeConverter<string, DateTime?>
    {
        public DateTime? Convert(string source, DateTime? destination, ResolutionContext context)
        {
            if (string.IsNullOrWhiteSpace(source))
                return null;

            DateTime.TryParseExact(source, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dest);
            return dest;
        }
    }
}