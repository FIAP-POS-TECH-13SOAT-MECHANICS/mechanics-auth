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
            Role = roleName,
            PasswordHash = "",
            SecurityStamp = userId.ToString(),
            CustomerId = Guid.NewGuid(),
        };
    }

    public static UserModel CreateUser(string userName, string userPassword)
    {
        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            CpfNumber = userName,
            Role = RoleNames.Administrator,
            PasswordHash = "",
            SecurityStamp = userName,
            CustomerId = Guid.NewGuid(),
        };

        user.PasswordHash = new PasswordHasher<UserModel>().HashPassword(user, userPassword);

        return user;
    }
}
