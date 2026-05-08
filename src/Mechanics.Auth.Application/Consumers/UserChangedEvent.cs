using Mechanics.Auth.Infra.Data.Models;

namespace Mechanics.Auth.Application.Consumers;

public class UserChangedEvent
{
    public required string Id { get; init; }
    public required string CpfNumber { get; init; }
    public required string FullName { get; init; }
    public required string Role { get; init; }
    public required string SecurityStamp { get; init; }
    public required string PasswordHash { get; init; }
    public string? CustomerId { get; init; }
    public DateTimeOffset LastUpdate { get; init; }

    public UserModel ToModel()
    {
        return new UserModel
        {
            Id = new Guid(Id),
            CpfNumber = CpfNumber,
            FullName = FullName,
            Role = Role,
            SecurityStamp = SecurityStamp,
            PasswordHash = PasswordHash,
            CustomerId = CustomerId is not null ? new Guid(CustomerId) : null,
            LastUpdate = LastUpdate.DateTime,
        };
    }
}
