namespace Mechanics.Auth.Infra.Data.CachedRepository;

public interface IRolesCachedRepository
{
    Task<string> GetRoleName(Guid roleId);
}
