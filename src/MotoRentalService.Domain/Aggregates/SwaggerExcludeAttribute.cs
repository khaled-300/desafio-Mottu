namespace MotoRentalService.Domain.Aggregates
{
    /// <summary>
    /// Attribute to exclude properties from Swagger documentation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class SwaggerExcludeAttribute : Attribute
    {
    }
}
