namespace Mechanics.Auth.Infra.Data.Repositories;

public static class UserQueries
{
    public const string GetById =
        """
            SELECT Id, CpfNumber, PasswordHash, SecurityStamp, CustomerId, RoleId
            FROM Mechanics.Users
            WHERE Id = @Id
        """;

    public const string GetByCpf =
        """
            SELECT Id, CpfNumber, PasswordHash, SecurityStamp, CustomerId, RoleId
            FROM Mechanics.Users
            WHERE CpfNumber = @Cpf
        """;
}
