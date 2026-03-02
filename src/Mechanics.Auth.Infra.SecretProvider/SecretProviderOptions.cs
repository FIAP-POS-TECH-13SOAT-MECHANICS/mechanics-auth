namespace Mechanics.Auth.Infra.SecretProvider;

public class SecretProviderOptions
{
    public required string PrivateKeySecretName { get; init; }
    public required string DbSecretName { get; init; }
}
