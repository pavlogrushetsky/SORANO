using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;

namespace SORANO.WEB.Infrastructure.Extensions
{
    public static class ModelStateExtensions
    {
        public static void RemoveFor(this ModelStateDictionary modelStateDictionary, string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            modelStateDictionary.Keys.Where(k => k.StartsWith(key))
                .ToList()
                .ForEach(k =>
                {
                    modelStateDictionary.Remove(k);
                });
        }
    }
}
