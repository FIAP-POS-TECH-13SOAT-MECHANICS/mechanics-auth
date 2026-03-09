namespace Mechanics.Auth.Application.Requests;

public class RefreshTokenRequest
{
    /// <summary>
    ///     Refresh token used for getting a new access token.
    /// </summary>
    public required string RefreshToken { get; init; }
}
