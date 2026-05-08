using Mechanics.Auth.Application.Requests;
using Mechanics.Auth.Application.Response;
using Mechanics.Auth.Application.TokenGenerator;
using Mechanics.Auth.Infra.Data.Models;
using Mechanics.Auth.Infra.Data.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Mechanics.Auth.Application.Services;

public class AuthAppService(ILogger<AuthAppService> logger, IUserRepository userRepository, IJwtTokenHandler tokenHandler)
    : IAppService
{
    public async Task<TokenResponse?> Login(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var normalizedCpf = new string(request.CpfNumber.Where(char.IsDigit).ToArray());
        if (normalizedCpf.Length != 11)
        {
            logger.LogWarning("Invalid CPF format");
            return null;
        }

        logger.LogTrace("Fetching user with CPF '{Cpf}'", normalizedCpf[..5]);
        var user = await userRepository.GetByCpf(normalizedCpf, cancellationToken);
        if (user is null)
        {
            logger.LogDebug("User with CPF '{Cpf}' not found", normalizedCpf[..5]);
            return null;
        }

        var passwordVerificationResult =
            new PasswordHasher<UserModel>().VerifyHashedPassword(user, user.PasswordHash, request.Password);
        logger.LogTrace("Password verification result: '{PasswordVerificationResult}'", passwordVerificationResult);

        return passwordVerificationResult is not PasswordVerificationResult.Failed
            ? await tokenHandler.CreateTokenResponse(user)
            : null;
    }

    public async Task<TokenResponse?> Refresh(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        var userId = tokenHandler.GetUserId(request.RefreshToken);
        if (userId is null)
            return null;

        logger.LogTrace("Fetching user with ID '{Id}'", userId);
        var user = await userRepository.GetById(userId.Value, cancellationToken);
        if (user is null || !await tokenHandler.ValidateRefreshToken(request.RefreshToken, user.SecurityStamp))
            return null;

        logger.LogTrace("Access token for user '{Id}' was refreshed", userId);
        return await tokenHandler.CreateTokenResponse(user);
    }

    public async Task<TokenResponse?> ServiceToken(string serviceName) =>
        await tokenHandler.CreateTokenResponse(serviceName);
}
