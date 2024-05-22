using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MotoRentalService.API.Binders
{
    public class EnumModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            ArgumentNullException.ThrowIfNull(bindingContext);
            var modelMetadata = bindingContext.ModelMetadata;
            Console.WriteLine($"ModelType: {modelMetadata.ModelType}, IsEnum: {modelMetadata.IsEnum}");

            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            var value = valueProviderResult.FirstValue;

            // Try binding by key (string)
            if (TryBindByKey(bindingContext, value, out var enumValue))
            {
                bindingContext.Result = ModelBindingResult.Success(enumValue);
                return Task.CompletedTask;
            }

            // If key binding fails, try binding by value (integer)
            if (TryBindByValue(bindingContext, value, out enumValue))
            {
                bindingContext.Result = ModelBindingResult.Success(enumValue);
                return Task.CompletedTask;
            }

            // Add error message if neither binding succeeds
            bindingContext.ModelState.AddModelError(bindingContext.ModelName, $"Invalid value '{value}' for enum type '{bindingContext.ModelType}'.");
            return Task.CompletedTask;
        }

        //private bool TryBindByKey(ModelBindingContext context, string value, out object enumValue)
        //{
        //    //var modelType = context.ModelType;
        //    //var underlyingType = Nullable.GetUnderlyingType(modelType) ?? modelType;
        //    //enumValue = Enum.Parse(underlyingType, value, true); // Ignore case
        //    //return enumValue != null;

        //    enumValue = null;
        //    var modelType = context.ModelType;
        //    var underlyingType = Nullable.GetUnderlyingType(modelType) ?? modelType;
        //    try
        //    {
        //        enumValue = Enum.Parse(underlyingType, value, true); // Ignore case
        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

        //private bool TryBindByValue(ModelBindingContext context, string value, out object? enumValue)
        //{
        //    if (int.TryParse(value, out var intValue))
        //    {
        //        var modelType = context.ModelType;
        //        var underlyingType = Nullable.GetUnderlyingType(modelType) ?? modelType;
        //        enumValue = Enum.ToObject(underlyingType, intValue);
        //        return enumValue != null;
        //    }

        //    enumValue = null;
        //    return false;
        //}

        private bool TryBindByKey(ModelBindingContext context, string value, out object enumValue)
        {
            enumValue = null;
            var modelType = context.ModelType;
            var underlyingType = Nullable.GetUnderlyingType(modelType) ?? modelType;

            // Handle empty value
            if (string.IsNullOrEmpty(value))
            {
                if (modelType.IsGenericType && modelType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    return true; // Return null for nullable enums
                }
                else
                {
                    return false; // Cannot bind empty value for non-nullable enums
                }
            }

            try
            {
                enumValue = Enum.Parse(underlyingType, value, true); // Ignore case
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool TryBindByValue(ModelBindingContext context, string value, out object? enumValue)
        {
            if (string.IsNullOrEmpty(value))
            {
                var modelType = context.ModelType;
                if (modelType.IsGenericType && modelType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    enumValue = null; // Return null for nullable enums
                    return true;
                }
                else
                {
                    enumValue = Enum.GetValues(modelType).GetValue(0); // Return first enum value for non-nullable enums
                    return true;
                }
            }

            if (int.TryParse(value, out var intValue))
            {
                var modelType = context.ModelType;
                var underlyingType = Nullable.GetUnderlyingType(modelType) ?? modelType;
                enumValue = Enum.ToObject(underlyingType, intValue);
                return true;
            }

            enumValue = null;
            return false;
        }
    }
}
