using Amazon.DynamoDBv2.DataModel;
using Mechanics.Auth.Infra.Data.Models;
using Mechanics.Auth.Infra.Data.Options;
using Microsoft.Extensions.Options;

namespace Mechanics.Auth.Infra.Data.Repositories;

public class UserRepository(IDynamoDBContext context, IOptions<TableNames> options) : IUserRepository
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
}
