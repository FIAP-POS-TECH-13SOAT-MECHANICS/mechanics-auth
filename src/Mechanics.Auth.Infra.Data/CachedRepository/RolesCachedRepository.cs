using Dapper;
using Mechanics.Auth.Infra.Data.Connection;
using Mechanics.Auth.Infra.Data.Models;
using Microsoft.Extensions.Logging;

namespace Mechanics.Auth.Infra.Data.CachedRepository;

public class RolesCachedRepository(ILogger<RolesCachedRepository> logger, IDbConnectionFactory connectionFactory)
    : IRolesCachedRepository
{
    private Task<Dictionary<Guid, string>>? _roles;

    public async Task<string> GetRoleName(Guid roleId)
    {
        await ReloadRoles();

        return _roles!.Result[roleId];
    }

    private async Task ReloadRoles() =>
        _roles ??= await GetAll().ContinueWith(async rolesTask =>
        {
            logger.LogInformation("Roles cache updated");

            var roles = await rolesTask;
            return roles.ToDictionary(role => role.Id, role => role.Name);
        });

    private async Task<IEnumerable<RoleModel>> GetAll()
    {
        using var connection = await connectionFactory.CreateConnection();

        return await connection.QueryAsync<RoleModel>("SELECT Id, Name FROM Mechanics.Roles");
    }
}
