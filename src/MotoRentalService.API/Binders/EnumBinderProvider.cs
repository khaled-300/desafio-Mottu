using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MotoRentalService.API.Binders
{
    public class EnumBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.Metadata.IsEnum)
            {
                return new EnumModelBinder();
            }

            return null;
        }
    }
}
