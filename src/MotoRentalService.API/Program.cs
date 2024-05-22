using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MotoRentalService.API.Binders;
using MotoRentalService.API.Filters;
using MotoRentalService.Application;
using MotoRentalService.Domain;
using MotoRentalService.Domain.Entities;
using MotoRentalService.Infrastructure;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.ModelBinderProviders.Insert(0, new EnumBinderProvider());

}).AddJsonOptions(options =>
{
    // Configure enum handling
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApplication()
    .AddInfrastructure(builder.Configuration)
.AddDomain(builder.Configuration);

// Jwt Configuration
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));
var jwtConfig = builder.Configuration.GetSection("JwtConfig").Get<JwtConfig>();

var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.SecretKey));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,

            IssuerSigningKey = signingKey,
            ValidIssuer = jwtConfig.Issuer,
            ValidAudience = jwtConfig.Audience,

        };
    });


// Configure Serilog
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", context.HostingEnvironment.ApplicationName)
    .WriteTo.Console()
    .WriteTo.File(
        formatter: new RenderedCompactJsonFormatter(),
        path: "logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        restrictedToMinimumLevel: LogEventLevel.Information
    ));

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Rental API", Version = "v1", Description = "web api to rent a motorcycle using .Net 8" });
    c.CustomSchemaIds(type => type.Name);
    c.IgnoreObsoleteActions();
    var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.GetName().Name.StartsWith("MotoRentalService")).ToList();
    if(assemblies != null && assemblies.Count > 0)
    {
        foreach (var assembly in assemblies)
        {
            if (assembly == null) continue;

            var types = assembly.GetExportedTypes();
            foreach (var type in types)
            {
                if (!type.IsClass)
                {
                    continue;
                }
                Log.Information($"Scanning type: {type.Name}");
            }
        }
    }
    c.SchemaFilter<SwaggerExcludeSchemaFilter>();
    c.OperationFilter<FormSchemaFilter>();

    //c.UseAllOfToExtendReferenceSchemas();

    //c.SchemaFilter<JsonIgnoreSchemaFilter>();
    //c.SchemaFilter<EnumSchemaFilter>();

    foreach (var file in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.xml"))
    {
        if (File.Exists(file))
        {
            c.IncludeXmlComments(file);
        }
    }

    // Set up the Bearer token authentication
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                    Scheme = "oauth2",
                    Name = "Bearer",
                    In = ParameterLocation.Header,

                },
                new List<string>()
            }
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Rental API");
    });
}

app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();
