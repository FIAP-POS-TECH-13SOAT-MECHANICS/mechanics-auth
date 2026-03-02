namespace Mechanics.Auth.Application.Requests;

public class RefreshTokenRequest
{
    public required string RefreshToken { get; init; }
}
