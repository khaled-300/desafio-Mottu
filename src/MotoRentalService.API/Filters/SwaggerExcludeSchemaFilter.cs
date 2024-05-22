using Microsoft.OpenApi.Models;
using MotoRentalService.Domain.Aggregates;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace MotoRentalService.API.Filters
{
    public class SwaggerExcludeSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema?.Properties == null)
            {
                return;
            }

            var excludedProperties = context.Type.GetProperties()
                .Where(prop => prop.GetCustomAttribute<SwaggerExcludeAttribute>() != null)
                .Select(prop => prop.Name);

            foreach (var propName in excludedProperties)
            {
                // The property names in OpenApiSchema.Properties dictionary are camelCased by default in Swashbuckle
                var nameToRemove = Char.ToLowerInvariant(propName[0]) + propName.Substring(1);
                if (schema.Properties.ContainsKey(nameToRemove))
                {
                    schema.Properties.Remove(nameToRemove);
                }
            }
        }
    }
}
