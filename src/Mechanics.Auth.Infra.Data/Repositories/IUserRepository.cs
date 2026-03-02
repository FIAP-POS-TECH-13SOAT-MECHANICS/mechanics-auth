using Mechanics.Auth.Infra.Data.Models;

namespace Mechanics.Auth.Infra.Data.Repositories;

public interface IUserRepository
{
    Task<UserModel?> GetById(Guid id);
    Task<UserModel?> GetByCpf(string cpf);
}
