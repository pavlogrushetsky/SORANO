using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;

namespace SORANO.WEB.Infrastructure.Binders
{
    public class DecimalModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            if (!context.Metadata.IsComplexType && (context.Metadata.ModelType == typeof(decimal) || context.Metadata.ModelType == typeof(decimal?)))
            {
                return new DecimalModelBinder(context.Metadata.ModelType);
            }

            return null;
        }
    }
}
