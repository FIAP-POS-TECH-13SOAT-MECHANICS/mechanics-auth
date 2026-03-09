using Mechanics.Auth.Infra.Data.CachedRepository;
using Mechanics.Auth.Infra.Data.Models;
using Mechanics.Auth.Tests.Unit.Mocks;

namespace Mechanics.Auth.Tests.Unit.Helpers;

public class StaticRolesRepository : IRolesCachedRepository
{
    private static readonly Dictionary<Guid, RoleModel> Roles = RoleMocks.Roles.ToDictionary(role => role.Id);

    public Task<string> GetRoleName(Guid roleId) =>
        Task.FromResult(Roles.TryGetValue(roleId, out var role) ? role.Name : string.Empty);
}
