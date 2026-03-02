using Mechanics.Auth.Application.Requests;
using Mechanics.Auth.Application.Response;
using Mechanics.Auth.Application.TokenGenerator;
using Mechanics.Auth.Infra.Data.Models;
using Mechanics.Auth.Infra.Data.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Mechanics.Auth.Application.Services;

public class AuthAppService(IUserRepository userRepository, IJwtTokenHandler tokenHandler) : IAppService
{
    public async Task<TokenResponse?> Login(LoginRequest request)
    {
        var normalizedCpf = new string(request.CpfNumber.Where(char.IsDigit).ToArray());

        var user = await userRepository.GetByCpf(normalizedCpf);
        if (user is null)
            return null;

        var passwordVerificationResult =
            new PasswordHasher<UserModel>().VerifyHashedPassword(user, user.PasswordHash, request.Password);
        return passwordVerificationResult is not PasswordVerificationResult.Failed
            ? await tokenHandler.CreateTokenResponse(user)
            : null;
    }

    public async Task<TokenResponse?> Refresh(RefreshTokenRequest request)
    {
        var userId = tokenHandler.GetUserId(request.RefreshToken);
        if (userId is null)
            return null;

        var user = await userRepository.GetById(userId.Value);
        if (user is null || !await tokenHandler.ValidateRefreshToken(request.RefreshToken, user.SecurityStamp))
            return null;

        return await tokenHandler.CreateTokenResponse(user);
    }
}
