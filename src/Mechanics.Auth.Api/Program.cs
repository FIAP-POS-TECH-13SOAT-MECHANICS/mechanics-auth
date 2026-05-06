using Mechanics.Auth.Api.Extensions;
using Mechanics.Auth.Infra.CrossCutting.IoC.Extensions;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Diagnostics.CodeAnalysis;

namespace Mechanics.Auth.Api;

public class Program
{
    [ExcludeFromCodeCoverage]
    protected Program()
    {
    }

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers(options =>
            options.Conventions.Add(new RouteTokenTransformerConvention(new KebabCaseParameterTransformer())));

        builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

        builder.Services.AddDataRepositories(builder.Configuration)
            .AddAppServices(builder.Configuration)
            .AddSecretProvider(builder.Configuration);

        builder.Services.AddGlobalCorsPolicy();

#if DEBUG
        builder.Services.AddSwaggerDocumentation();
#endif

        var app = builder.Build();

        app.UseCors("AllowAllOrigins");
        app.MapControllers();

#if DEBUG
        app.UseSwaggerDocumentation();
#endif

        app.Run();
    }
}
