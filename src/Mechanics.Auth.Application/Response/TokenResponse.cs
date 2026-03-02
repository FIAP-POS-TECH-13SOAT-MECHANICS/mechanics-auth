namespace Mechanics.Auth.Application.Response;

public record TokenResponse
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
    public required int ExpiresIn { get; init; }
    public required DateTimeOffset ExpirationDate { get; init; }
}
