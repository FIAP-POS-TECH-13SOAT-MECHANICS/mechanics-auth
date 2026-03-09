namespace Mechanics.Auth.Infra.Data.Models;

public class UserModel
{
    public required Guid Id { get; init; }
    public required string CpfNumber { get; init; }
    public required string PasswordHash { get; set; }
    public required string SecurityStamp { get; init; }
    public Guid? CustomerId { get; init; }
    public required Guid RoleId { get; init; }
}
