using Mechanics.Auth.Infra.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace Mechanics.Auth.Tests.Unit.Mocks;

public static class UserMocks
{
    public static UserModel CreateUser(Guid userId, string cpf, string roleName)
    {
        return new UserModel
        {
            Id = userId,
            CpfNumber = cpf,
            FullName = roleName,
            Role = roleName,
            PasswordHash = "",
            SecurityStamp = userId.ToString(),
            CustomerId = Guid.NewGuid(),
            LastUpdate = new DateTime(2025, 10, 12, 12, 0, 0, DateTimeKind.Utc),
        };
    }

    public static UserModel CreateUser(string userName, string userPassword)
    {
        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            CpfNumber = userName,
            FullName = $"{RoleNames.Administrator} USER",
            Role = RoleNames.Administrator,
            PasswordHash = "",
            SecurityStamp = userName,
            CustomerId = Guid.NewGuid(),
            LastUpdate = new DateTime(2025, 10, 12, 14, 0, 0, DateTimeKind.Utc),
        };

        user.PasswordHash = new PasswordHasher<UserModel>().HashPassword(user, userPassword);

        return user;
    }
}
