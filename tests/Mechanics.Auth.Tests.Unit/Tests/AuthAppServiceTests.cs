using Mechanics.Auth.Application.Requests;
using Mechanics.Auth.Application.Response;
using Mechanics.Auth.Application.Services;
using Mechanics.Auth.Application.TokenGenerator;
using Mechanics.Auth.Infra.Data.Models;
using Mechanics.Auth.Infra.Data.Repositories;
using Mechanics.Auth.Tests.Unit.Mocks;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Mechanics.Auth.Tests.Unit.Tests;

[TestClass]
[TestCategory("AppService")]
public class AuthAppServiceTests
{
    #region efetuar login

    [TestMethod("Efetuar login com usuário válido deve retornar token")]
    public async Task It_ShouldReturnToken_WithValidCredentials()
    {
        var token = Guid.NewGuid().ToString();
        var tokenHandlerStub = CreateTokenHandlerStub(token);
        var user = UserMocks.CreateUser("12345678909", "TEST_5eCre+Key");
        var repository = CreateUserRepository(user);
        var appService = new AuthAppService(NullLogger<AuthAppService>.Instance, repository.Object, tokenHandlerStub.Object);
        var request = new LoginRequest
        {
            CpfNumber = "12345678909",
            Password = "TEST_5eCre+Key",
        };

        var result = await appService.Login(request);

        Assert.IsNotNull(result);
        Assert.AreEqual(token, result.AccessToken);
        Assert.IsNotNull(result.RefreshToken);
    }

    [TestMethod("Efetuar login com senha incorreta deve retornar null")]
    public async Task It_ShouldReturnNull_WithInvalidCredentials()
    {
        var token = Guid.NewGuid().ToString();
        var tokenHandlerStub = CreateTokenHandlerStub(token);
        var user = UserMocks.CreateUser("12345678909", "TEST_5eCre+Key");
        var repository = CreateUserRepository(user);
        var appService = new AuthAppService(NullLogger<AuthAppService>.Instance, repository.Object, tokenHandlerStub.Object);
        var request = new LoginRequest
        {
            CpfNumber = "12345678909",
            Password = "wrong-password",
        };

        var result = await appService.Login(request);

        Assert.IsNull(result);
    }

    #endregion

    #region Renovar token

    [TestMethod("Renovar com refresh token válido")]
    public async Task It_ShouldReturnToken_WithValidRefreshToken()
    {
        var userId = new Guid("5bb2ae44-cbc7-44c4-9eda-cfb860b6e2f5");
        var user = UserMocks.CreateUser(userId, "38446983028", RoleMocks.Names.Mechanic);
        var token = Guid.NewGuid().ToString();
        var refreshToken = Guid.NewGuid().ToString();
        var tokenHandlerStub = CreateTokenHandlerStub(token, refreshToken, userId.ToString(), userId);
        var repository = CreateUserRepository(user);
        var appService = new AuthAppService(NullLogger<AuthAppService>.Instance, repository.Object, tokenHandlerStub.Object);
        var request = new RefreshTokenRequest { RefreshToken = refreshToken };

        var result = await appService.Refresh(request);

        Assert.IsNotNull(result);
        Assert.AreEqual(token, result.AccessToken);
        Assert.IsNotNull(result.RefreshToken);
    }

    #endregion

    #region Mocks

    private static Mock<IUserRepository> CreateUserRepository(UserModel user)
    {
        var userRepository = new Mock<IUserRepository>();

        userRepository.Setup(repository => repository.GetById(user.Id)).ReturnsAsync(user);
        userRepository.Setup(repository => repository.GetByCpf(user.CpfNumber)).ReturnsAsync(user);

        return userRepository;
    }

    private static Mock<IJwtTokenHandler> CreateTokenHandlerStub(string? accessToken = null, string? refreshToken = null,
        string? securityStamp = null, Guid? userId = null)
    {
        var tokenHandler = new Mock<IJwtTokenHandler>();
        tokenHandler.Setup(handler => handler.GetUserId(refreshToken ?? Guid.NewGuid().ToString())).Returns(userId);
        tokenHandler.Setup(handler => handler.CreateTokenResponse(It.IsAny<UserModel>())).ReturnsAsync(new TokenResponse
        {
            AccessToken = accessToken ?? Guid.NewGuid().ToString(),
            RefreshToken = refreshToken ?? Guid.NewGuid().ToString(),
            ExpiresIn = 3600,
            ExpirationDate = new DateTime(2025, 10, 15, 10, 0, 0, DateTimeKind.Utc),
        });
        tokenHandler.Setup(handler =>
                handler.ValidateRefreshToken(refreshToken ?? Guid.NewGuid().ToString(), securityStamp ?? Guid.NewGuid().ToString()))
            .ReturnsAsync(true);
        tokenHandler.Setup(handler => handler.GetUserId(refreshToken ?? Guid.NewGuid().ToString()))
            .Returns(userId ?? Guid.NewGuid());

        return tokenHandler;
    }

    #endregion
}
