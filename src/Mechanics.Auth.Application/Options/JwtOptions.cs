namespace Mechanics.Auth.Application.Options;

public class JwtOptions
{
    /// <summary>
    ///     Validade do token em minutos.
    /// </summary>
    public required int AccessTokenLifetime { get; init; }

    /// <summary>
    ///     Validade do token em minutos.
    /// </summary>
    public required int RefreshTokenLifetime { get; init; }
}
