using Mechanics.Auth.Application.Options;
using Mechanics.Auth.Application.Response;
using Mechanics.Auth.Infra.Data.CachedRepository;
using Mechanics.Auth.Infra.Data.Models;
using Mechanics.Auth.Infra.SecretProvider;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace Mechanics.Auth.Application.TokenGenerator;

public class JwtTokenHandler(
    IOptions<JwtOptions> jwtOptions,
    TimeProvider timeProvider,
    ISecretProvider secretProvider,
    IRolesCachedRepository rolesRepository)
    : IJwtTokenHandler
{
    private readonly JwtOptions _options = jwtOptions.Value;
    private readonly JsonWebTokenHandler _tokenHandler = new();
    private const string JwtTokenIssuer = "fiap-mechanics";
    private const string RefreshTokenSalt = "ef932c68c005c43607b2e076ced1472c";

    public Guid? GetUserId(string token)
    {
        if (!_tokenHandler.CanReadToken(token))
            return null;

        var subject = _tokenHandler.ReadJsonWebToken(token).Subject;
        return Guid.TryParse(subject, out var userId) ? userId : null;
    }

    public async Task<TokenResponse> CreateTokenResponse(UserModel user)
    {
        var expiration = timeProvider.GetUtcNow().AddMinutes(_options.AccessTokenLifetime);

        return new TokenResponse
        {
            AccessToken = await GenerateAccessToken(user, expiration),
            RefreshToken = GenerateRefreshToken(user),
            ExpiresIn = (int)expiration.Subtract(timeProvider.GetUtcNow()).TotalSeconds,
            ExpirationDate = expiration,
        };
    }

    public async Task<TokenResponse?> CreateTokenResponse(string serviceName)
    {
        var expiration = timeProvider.GetUtcNow().AddMinutes(_options.AccessTokenLifetime);
        var serviceId = _options.ServiceIds.TryGetValue(serviceName, out var id) ? id : _options.ServiceIds["default"];

        var claims = new List<Claim>
        {
            new("sub", serviceId.ToString()),
            new("customerId", Guid.Empty.ToString()),
            new("role", "SERVICE"), // TODO criar role para serviços
        };

        var privateKey = await secretProvider.GetPrivateKey();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = JwtTokenIssuer,
            Subject = new ClaimsIdentity(claims),
            Expires = expiration.UtcDateTime,
            SigningCredentials = new SigningCredentials(new RsaSecurityKey(privateKey), SecurityAlgorithms.RsaSha256),
            IssuedAt = timeProvider.GetUtcNow().UtcDateTime,
            NotBefore = timeProvider.GetUtcNow().UtcDateTime,
        };

        return new TokenResponse
        {
            AccessToken = _tokenHandler.CreateToken(tokenDescriptor),
            RefreshToken = string.Empty,
            ExpiresIn = (int)expiration.Subtract(timeProvider.GetUtcNow()).TotalSeconds,
            ExpirationDate = expiration,
        };
    }

    public async Task<bool> ValidateRefreshToken(string refreshToken, string securityStamp)
    {
        if (!_tokenHandler.CanReadToken(refreshToken))
            return false;
        var userId = _tokenHandler.ReadJsonWebToken(refreshToken).Subject;

        var refreshTokenKey = Encoding.ASCII.GetBytes($"{userId}:{securityStamp}:{RefreshTokenSalt}");
        var validationResult = await _tokenHandler.ValidateTokenAsync(refreshToken, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(refreshTokenKey),
            ValidateIssuer = true,
            ValidIssuer = JwtTokenIssuer,
            ValidateAudience = false,
            ValidateLifetime = true,
        });

        return validationResult.IsValid;
    }

    private async Task<string> GenerateAccessToken(UserModel user, DateTimeOffset expiration)
    {
        var claims = new List<Claim>
        {
            new("sub", user.Id.ToString()),
            new("customerId", (user.CustomerId ?? Guid.Empty).ToString()),
            new("role", await rolesRepository.GetRoleName(user.RoleId)),
        };

        var privateKey = await secretProvider.GetPrivateKey();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = JwtTokenIssuer,
            Subject = new ClaimsIdentity(claims),
            Expires = expiration.UtcDateTime,
            SigningCredentials = new SigningCredentials(new RsaSecurityKey(privateKey), SecurityAlgorithms.RsaSha256),
            IssuedAt = timeProvider.GetUtcNow().UtcDateTime,
            NotBefore = timeProvider.GetUtcNow().UtcDateTime,
        };

        return _tokenHandler.CreateToken(tokenDescriptor);
    }

    private string GenerateRefreshToken(UserModel user)
    {
        var claims = new List<Claim> { new("sub", user.Id.ToString()) };

        var key = Encoding.ASCII.GetBytes($"{user.Id}:{user.SecurityStamp}:{RefreshTokenSalt}");
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = JwtTokenIssuer,
            Subject = new ClaimsIdentity(claims),
            Expires = timeProvider.GetUtcNow().UtcDateTime.AddMinutes(_options.RefreshTokenLifetime),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
            IssuedAt = timeProvider.GetUtcNow().UtcDateTime,
        };

        return _tokenHandler.CreateToken(tokenDescriptor);
    }
}
