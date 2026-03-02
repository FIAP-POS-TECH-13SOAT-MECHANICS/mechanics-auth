using Mechanics.Auth.Application.Response;
using Mechanics.Auth.Infra.Data.Models;

namespace Mechanics.Auth.Application.TokenGenerator;

public interface IJwtTokenHandler
{
    Guid? GetUserId(string token);
    Task<bool> ValidateRefreshToken(string refreshToken, string securityStamp);
    Task<TokenResponse> CreateTokenResponse(UserModel user);
}
