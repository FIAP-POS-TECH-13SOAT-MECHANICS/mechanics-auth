using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Mechanics.Auth.Infra.Data.Models;
using Mechanics.Auth.Infra.Data.Options;
using Microsoft.Extensions.Options;

namespace Mechanics.Auth.Infra.Data.Repositories;

public class UserRepository(IDynamoDBContext context, IAmazonDynamoDB client, IOptions<TableNames> options) : IUserRepository
{
    private readonly string _tableName = options.Value.Users;

    public async Task<UserModel?> GetById(Guid id, CancellationToken cancellationToken)
    {
        return await context.LoadAsync<UserModel>(id, new LoadConfig { OverrideTableName = _tableName }, cancellationToken);
    }

    public async Task<UserModel?> GetByCpf(string cpf, CancellationToken cancellationToken)
    {
        var search = context.QueryAsync<UserModel>(QueryConditional.HashKeyEqualTo("cpfNumber", cpf),
            new QueryConfig { OverrideTableName = _tableName, IndexName = "cpfNumber-index" });

        var results = await search.GetRemainingAsync(cancellationToken);
        return results.FirstOrDefault();
    }

    public async Task Upsert(UserModel user, CancellationToken cancellationToken = default)
    {
        var request = new PutItemRequest
        {
            TableName = _tableName,
            ConditionExpression = "attribute_not_exists(id) OR lastUpdate < :incoming",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                [":incoming"] = new() { S = user.LastUpdate.ToString("O") },
            },
            Item = new Dictionary<string, AttributeValue>
            {
                ["id"] = new() { S = user.Id.ToString() },
                ["cpfNumber"] = new() { S = user.CpfNumber },
                ["fullName"] = new() { S = user.FullName },
                ["role"] = new() { S = user.Role },
                ["securityStamp"] = new() { S = user.SecurityStamp },
                ["passwordHash"] = new() { S = user.PasswordHash },
                ["lastUpdate"] = new() { S = user.LastUpdate.ToString("O") },
                ["customerId"] = user.CustomerId is not null
                    ? new AttributeValue { S = user.CustomerId.ToString() }
                    : new AttributeValue { NULL = true },
            },
        };

        await client.PutItemAsync(request, cancellationToken);
    }
}
