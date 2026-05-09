using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.Lambda.SQSEvents;
using Mechanics.Auth.Application.Consumers;
using Mechanics.Auth.Infra.CrossCutting.IoC.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

[assembly: LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]

namespace Mechanics.Auth.Consumer;

public class Function
{
    private readonly UserChangedEventConsumer _service = CreateService();

    public async Task<SQSBatchResponse> Handler(SQSEvent sqsEvent, ILambdaContext context)
    {
        var response = new SQSBatchResponse { BatchItemFailures = [] };

        foreach (var record in sqsEvent.Records)
        {
            try
            {
                var message = JsonSerializer.Deserialize<UserChangedEvent>(record.Body)!;
                await _service.Save(message);
            }
            catch (ConditionalCheckFailedException)
            {
                using var document = JsonDocument.Parse(record.Body);
                var userId = document.RootElement.GetProperty("Id").GetString();

                context.Logger.LogWarning("User with '{Id}' has a newer persisted entry and was ignored", userId);
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Failed to process message {record.MessageId}: {ex.Message}");
                response.BatchItemFailures.Add(new SQSBatchResponse.BatchItemFailure { ItemIdentifier = record.MessageId });
            }
        }

        return response;
    }

    private static UserChangedEventConsumer CreateService()
    {
        var services = new ServiceCollection();
        services.AddDataRepositories()
            .AddEventConsumers();

        var provider = services.BuildServiceProvider();
        return provider.GetRequiredService<UserChangedEventConsumer>();
    }
}
