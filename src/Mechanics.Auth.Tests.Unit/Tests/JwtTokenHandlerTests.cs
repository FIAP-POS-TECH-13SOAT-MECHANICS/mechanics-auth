using Mechanics.Auth.Application.Options;
using Mechanics.Auth.Application.TokenGenerator;
using Mechanics.Auth.Tests.Unit.Helpers;
using Mechanics.Auth.Tests.Unit.Mocks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;
using System.Security.Cryptography;

namespace Mechanics.Auth.Tests.Unit.Tests;

[TestClass]
[TestCategory("Token")]
public class JwtTokenHandlerTests
{
    [TestMethod("Deve gerar token")]
    public async Task It_ShouldGenerateToken()
    {
        var user = UserMocks.CreateUser("47a67d29-ebe9eb190d97", "vmlK5NcD");
        var handler = CreateInstance(TimeProvider.System);

        var token = await handler.CreateTokenResponse(user);

        Assert.IsNotNull(token);
        Assert.IsFalse(string.IsNullOrWhiteSpace(token.AccessToken));
        Assert.IsFalse(string.IsNullOrWhiteSpace(token.RefreshToken));
    }

    [TestMethod("Deve extrair ID do token")]
    public async Task It_ShouldExtractUserIdFromToken()
    {
        var user = UserMocks.CreateUser("bb4c7b7e-f65865670979", "2aQuV6WCl9O");
        var handler = CreateInstance(TimeProvider.System);
        var token = await handler.CreateTokenResponse(user);

        var userId = handler.GetUserId(token.AccessToken);

        Assert.IsNotNull(userId);
        Assert.AreEqual(user.Id, userId);
    }

    [TestMethod("Deve retornar true para refresh token válido")]
    public async Task It_ShouldReturnTrue_WhenValidateRefreshToken()
    {
        var user = UserMocks.CreateUser("bb4c7b7e-f65865670979", "2aQuV6WCl9O");
        var handler = CreateInstance(TimeProvider.System);
        var token = await handler.CreateTokenResponse(user);

        var isValid = await handler.ValidateRefreshToken(token.RefreshToken, user.SecurityStamp);

        Assert.IsTrue(isValid);
    }

    [TestMethod("Deve retornar false para securityStamp inválido")]
    public async Task It_ShouldReturnFalse_WhenValidateRefreshToken_WhitInvalidSecurityStamp()
    {
        var user = UserMocks.CreateUser("bb4c7b7e-f65865670979", "iKCg6koUvMr");
        var handler = CreateInstance(TimeProvider.System);
        var token = await handler.CreateTokenResponse(user);

        var isValid = await handler.ValidateRefreshToken(token.RefreshToken, "DOo0rUV");

        Assert.IsFalse(isValid);
    }

    [TestMethod("Deve retornar false para refresh token expirado")]
    public async Task It_ShouldReturnFalse_WhenRefreshTokenIsExpired()
    {
        var user = UserMocks.CreateUser("bb4c7b7e-f65865670979", "iKCg6koUvMr");
        var handler = CreateInstance(new FakeTimeProvider(DateTimeOffset.UnixEpoch));
        var token = await handler.CreateTokenResponse(user);

        var isValid = await handler.ValidateRefreshToken(token.RefreshToken, user.SecurityStamp);

        Assert.IsFalse(isValid);
    }

    private static JwtTokenHandler CreateInstance(TimeProvider timeProvider)
    {
        var options = new OptionsWrapper<JwtOptions>(new JwtOptions
        {
            AccessTokenLifetime = 10,
            RefreshTokenLifetime = 120,
        });

        var rsa = RSA.Create();
        var secretProvider = new StaticSecretProvider(rsa);

        return new JwtTokenHandler(options, timeProvider, secretProvider, new StaticRolesRepository());
    }
}
