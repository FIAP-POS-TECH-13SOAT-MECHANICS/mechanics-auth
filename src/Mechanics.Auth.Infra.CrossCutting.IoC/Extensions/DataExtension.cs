using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using Mechanics.Auth.Infra.Data.Options;
using Mechanics.Auth.Infra.Data.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mechanics.Auth.Infra.CrossCutting.IoC.Extensions;

public static class DataExtension
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddDataRepositories(IConfiguration configuration)
        {
            services.AddSingleton<IAmazonDynamoDB>(_ =>
            {
                var options = configuration.GetSection("AwsCredentials").Get<AwsCredentialsOptions>()!;

                if (options.UseLocalstack)
                    return new AmazonDynamoDBClient(
                        new BasicAWSCredentials("local", "empty-key"),
                        new AmazonDynamoDBConfig
                        {
                            RegionEndpoint = RegionEndpoint.GetBySystemName(options.Region),
                            ServiceURL = options.LocalstackUrl,
                        });

                return new AmazonDynamoDBClient(new AmazonDynamoDBConfig());
            });

            services.AddSingleton<IDynamoDBContext, DynamoDBContext>();
            services.Configure<TableNames>(configuration.GetSection(nameof(TableNames)));

            services.AddScoped<IUserRepository, UserRepository>();

            return services;
        }

        public IServiceCollection AddDataRepositories()
        {
            services.AddSingleton<IAmazonDynamoDB, AmazonDynamoDBClient>();
            services.AddSingleton<IDynamoDBContext, DynamoDBContext>();

            services.Configure<TableNames>(options =>
                options.Users = Environment.GetEnvironmentVariable("TableNames__Users")!);

            services.AddScoped<IUserRepository, UserRepository>();

            return services;
        }
    }
}
