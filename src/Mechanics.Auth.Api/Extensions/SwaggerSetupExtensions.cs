using Mechanics.Auth.Application;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;

namespace Mechanics.Auth.Api.Extensions;

public static class SwaggerSetupExtensions
{
    public static void AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Mechanics Auth API",
                Version = "v1",
                Description = "API para gestão de tokens JWT.",
            });

            var apiXmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, apiXmlFile), includeControllerXmlComments: true);
            var applicationXmlFile = new FileInfo(typeof(IAppService).Assembly.Location);
            c.IncludeXmlComments(Path.Combine(applicationXmlFile.DirectoryName!,
                applicationXmlFile.Name.Replace("dll", "xml")));

            c.EnableAnnotations();
            c.ExampleFilters();
        });

        services.AddSwaggerExamplesFromAssemblies(Assembly.GetExecutingAssembly());
    }

    public static void UseSwaggerDocumentation(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Mechanics Auth v1");
            c.RoutePrefix = "swagger";
        });
    }
}
