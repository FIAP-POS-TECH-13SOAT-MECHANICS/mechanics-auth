using Mechanics.Auth.Infra.CrossCutting.IoC.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

builder.Services.AddDataRepositories()
    .AddAppServices(builder.Configuration)
    .AddSecretProvider(builder.Configuration);

var app = builder.Build();

app.MapControllers();

app.MapGet("/", () => "Welcome to running ASP.NET Core Minimal API on AWS Lambda");

app.Run();
