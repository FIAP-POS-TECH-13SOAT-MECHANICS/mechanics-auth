using Mechanics.Auth.Infra.CrossCutting.IoC.Extensions;
using Mechanics.Auth.Infra.SecretProvider;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSecretProvider(builder.Configuration);

var app = builder.Build();

app.MapGet("/hello", () => Results.Ok(new
{
    message = "Hello from Mechanics.Auth",
    timestamp = DateTime.UtcNow,
}));

app.MapGet("/private", async (ISecretProvider secretProvider) => Results.Ok(new
{
    Key = await secretProvider.GetPrivateKey(),
}));

app.Run();

public partial class Program;
