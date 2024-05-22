using Microsoft.OpenApi.Models;
using MotoRentalService.Domain.Aggregates;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace MotoRentalService.API.Filters
{
    public class FormSchemaFilter : IOperationFilter
    {

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var mediaTypes = new[] { "application/x-www-form-urlencoded", "multipart/form-data" };

            if (operation.RequestBody?.Content.Any(x => mediaTypes.Contains(x.Key)) == true)
            {
                foreach (var content in operation.RequestBody.Content.Where(x => mediaTypes.Contains(x.Key)))
                {
                    var formDataType = content.Value.Schema?.Type;
                    if (formDataType != null && formDataType == "object")
                    {
                        var properties = context.ApiDescription.ParameterDescriptions
                            .SelectMany(p => p.ParameterDescriptor.ParameterType.GetProperties())
                            .Distinct();

                        var className = GetClassName(properties.First().DeclaringType);

                        // Check if the schema already exists in the SchemaRepository to prevent duplicate keys
                        if (!context.SchemaRepository.Schemas.ContainsKey(className))
                        {
                            var schema = new OpenApiSchema
                            {
                                Type = "object",
                                Properties = new Dictionary<string, OpenApiSchema>()
                            };

                            foreach (var prop in properties)
                            {
                                if (!prop.IsDefined(typeof(SwaggerExcludeAttribute)))
                                {
                                    schema.Properties[prop.Name] = context.SchemaGenerator.GenerateSchema(prop.PropertyType, context.SchemaRepository);
                                }
                            }

                            // Add the schema to the SchemaRepository
                            context.SchemaRepository.AddDefinition(className, schema);

                            // Update the Content with the new schema reference
                            content.Value.Schema = new OpenApiSchema
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.Schema,
                                    Id = className
                                }
                            };
                        }
                        else
                        {
                            // Update the Content with the existing schema reference
                            content.Value.Schema = new OpenApiSchema
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.Schema,
                                    Id = className
                                }
                            };
                        }
                    }
                }
            }
        }

        private string GetClassName(Type type)
        {
            return type.Name;
        }
    }
}

