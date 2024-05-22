using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MotoRentalService.API.Filters
{
    public class EnumSchemaFilter : ISchemaFilter
    {
        //public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        //{
        //    var type = context.Type;
        //    if (type.IsEnum)
        //    {
        //        schema.Enum.Clear();
        //        foreach (var name in Enum.GetNames(type))
        //        {
        //            var memberInfos = type.GetMember(name);
        //            var enumValue = (int)Enum.Parse(type, name);
        //            if (memberInfos.Length > 0)
        //            {
        //                var value = (int)type.GetField(name).GetValue(null);
        //                schema.Enum.Add(new OpenApiString($"{name} = {value}"));
        //            }
        //        }
        //        schema.Type = "string";
        //        schema.Description += " (Enum as key-value)";
        //    }
        //}
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type.IsEnum)
            {
                var enumType = context.Type;
                var names = Enum.GetNames(enumType);

                schema.Description = $"Enumeration representing {schema.Description}";
                schema.Enum.Clear();

                for (int i = 0; i < names.Length; i++)
                {
                    schema.Enum.Add(new OpenApiString(names[i]));
                }

                // Remove default integer format
                schema.Format = null;
            }
        }
    }
}
