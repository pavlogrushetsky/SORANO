using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace SORANO.WEB.Infrastructure.Binders
{
    public class DecimalModelBinder : IModelBinder
    {
        private readonly SimpleTypeModelBinder _baseBinder;

        public DecimalModelBinder(Type modelType)
        {
            _baseBinder = new SimpleTypeModelBinder(modelType);
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null) throw new ArgumentNullException(nameof(bindingContext));

            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (valueProviderResult != ValueProviderResult.None)
            {
                bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

                var valueAsString = valueProviderResult.FirstValue;

                // Use invariant culture
                if (decimal.TryParse(valueAsString, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal result))
                {
                    bindingContext.Result = ModelBindingResult.Success(result);
                    return Task.FromResult(0);
                }
            }

            return _baseBinder.BindModelAsync(bindingContext);
        }
    }
}
