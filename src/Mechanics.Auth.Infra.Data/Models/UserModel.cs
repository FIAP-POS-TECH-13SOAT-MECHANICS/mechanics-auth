using Amazon.DynamoDBv2.DataModel;
using Mechanics.Auth.Infra.Data.TypeConverters;

namespace Mechanics.Auth.Infra.Data.Models;

[DynamoDBTable(nameof(UserModel), LowerCamelCaseProperties = true)]
public class UserModel
{
    [DynamoDBHashKey(typeof(GuidTypeConverter))]
    public required Guid Id { get; init; }

    [DynamoDBGlobalSecondaryIndexHashKey]
    public required string CpfNumber { get; init; }

    [DynamoDBProperty]
    public required string PasswordHash { get; set; }

    [DynamoDBProperty]
    public required string SecurityStamp { get; init; }

    [DynamoDBProperty(typeof(GuidTypeConverter))]
    public Guid? CustomerId { get; init; }

    [DynamoDBProperty]
    public required string Role { get; init; }
}
