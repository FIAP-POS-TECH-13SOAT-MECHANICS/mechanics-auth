namespace Mechanics.Auth.Infra.Data.Options;

public class AwsCredentialsOptions
{
    public required string Region { get; init; }
    public bool UseLocalstack { get; init; }
    public string? LocalstackUrl { get; init; }
}
