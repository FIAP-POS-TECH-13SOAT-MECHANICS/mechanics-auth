using Mechanics.Auth.Infra.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace Mechanics.Auth.Tests.Unit.Mocks;

public static class UserMocks
{
    private static readonly Dictionary<string, RoleModel> Roles = RoleMocks.Roles.ToDictionary(role => role.Name);

    public static UserModel CreateUser(Guid userId, string cpf, string roleName)
    {
        var role = Roles[roleName];

        return new UserModel
        {
            Id = userId,
            CpfNumber = cpf,
            RoleId = role.Id,
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
            RoleId = Roles[RoleMocks.Names.Administrator].Id,
            PasswordHash = "",
            SecurityStamp = userName,
            CustomerId = Guid.NewGuid(),
        };

        user.PasswordHash = new PasswordHasher<UserModel>().HashPassword(user, userPassword);

        return user;
    }
}
