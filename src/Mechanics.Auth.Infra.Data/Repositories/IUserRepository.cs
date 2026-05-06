using Mechanics.Auth.Infra.Data.Models;

namespace Mechanics.Auth.Infra.Data.Repositories;

public interface IUserRepository
{
    Task<UserModel?> GetById(Guid id, CancellationToken cancellationToken = default);
    Task<UserModel?> GetByCpf(string cpf, CancellationToken cancellationToken = default);
}
