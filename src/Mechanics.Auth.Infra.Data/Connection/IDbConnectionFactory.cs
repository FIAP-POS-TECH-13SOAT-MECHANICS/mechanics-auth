using System.Data;

namespace Mechanics.Auth.Infra.Data.Connection;

public interface IDbConnectionFactory
{
    Task<IDbConnection> CreateConnection();
}
