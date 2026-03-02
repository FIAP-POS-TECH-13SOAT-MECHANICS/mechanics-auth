using Dapper;
using Mechanics.Auth.Infra.Data.Connection;
using Mechanics.Auth.Infra.Data.Models;

namespace Mechanics.Auth.Infra.Data.Repositories;

public class UserRepository(IDbConnectionFactory connectionFactory) : IUserRepository
{
    public async Task<UserModel?> GetById(Guid id)
    {
        using var connection = await connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<UserModel>(UserQueries.GetById, new { Id = id });
    }

    public async Task<UserModel?> GetByCpf(string cpf)
    {
        using var connection = await connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<UserModel>(UserQueries.GetByCpf, new { Cpf = cpf });
    }
}
