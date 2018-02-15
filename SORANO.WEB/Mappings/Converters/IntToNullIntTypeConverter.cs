using AutoMapper;

namespace SORANO.WEB.Mappings.Converters
{
    public class IntToNullIntTypeConverter : ITypeConverter<int, int?>
    {
        public int? Convert(int source, int? destination, ResolutionContext context)
        {
            return source == 0 ? (int?) null : source;
        }
    }
}