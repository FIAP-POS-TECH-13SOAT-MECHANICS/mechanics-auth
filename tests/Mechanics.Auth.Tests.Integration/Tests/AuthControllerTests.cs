using Mechanics.Auth.Application.Requests;
using Mechanics.Auth.Application.Response;
using Mechanics.Auth.Tests.Integration.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;

namespace Mechanics.Auth.Tests.Integration.Tests;

[TestClass]
[TestCategory("Auth")]
public class AuthControllerTests
{
    public TestContext TestContext { get; set; }
    private readonly HttpClient _client = TestProperties.Factory.CreateClient();

    #region Login

    [TestMethod]
    public async Task Login_WithValidCredentials_ReturnsOk()
    {
        // Arrange
        var request = new LoginRequest
        {
            CpfNumber = "12345678909",
            Password = "5eCre+Key",
        };

        // Act
        var response = await _client.PostAsJsonAsync("api/auth/login", request, TestContext.CancellationTokenSource.Token);

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>(TestContext.CancellationTokenSource.Token);
        Assert.IsNotNull(tokenResponse);
        Assert.IsFalse(string.IsNullOrWhiteSpace(tokenResponse.AccessToken));
        Assert.IsFalse(string.IsNullOrWhiteSpace(tokenResponse.RefreshToken));
    }


    [TestMethod]
    public async Task Login_WithMaskedCpf_ReturnsOk()
    {
        // Arrange
        var request = new LoginRequest
        {
            CpfNumber = "123.456.789-09",
            Password = "5eCre+Key",
        };

        // Act
        var response = await _client.PostAsJsonAsync("api/auth/login", request, TestContext.CancellationTokenSource.Token);

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>(TestContext.CancellationTokenSource.Token);
        Assert.IsNotNull(tokenResponse);
        Assert.IsFalse(string.IsNullOrWhiteSpace(tokenResponse.AccessToken));
        Assert.IsFalse(string.IsNullOrWhiteSpace(tokenResponse.RefreshToken));
    }

    [TestMethod]
    public async Task Login_WithInvalidPassword_ReturnsUnauthorized()
    {
        // Arrange
        var request = new LoginRequest
        {
            CpfNumber = "12345678909",
            Password = "WrongPassword",
        };

        // Act
        var response = await _client.PostAsJsonAsync("api/auth/login", request, TestContext.CancellationTokenSource.Token);

        // Assert
        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [TestMethod]
    public async Task Login_WithInvalidUser_ReturnsUnauthorized()
    {
        // Arrange
        var request = new LoginRequest
        {
            CpfNumber = "38470024060",
            Password = "5eCre+Key",
        };

        // Act
        var response = await _client.PostAsJsonAsync("api/auth/login", request, TestContext.CancellationTokenSource.Token);

        // Assert
        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [TestMethod]
    public async Task Login_ForCustomerUser_MustContainsCustomerId()
    {
        // Arrange
        var request = new LoginRequest
        {
            CpfNumber = "90526359005",
            Password = "5eCre+Key",
        };

        // Act
        var response = await _client.PostAsJsonAsync("api/auth/login", request, TestContext.CancellationTokenSource.Token);

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>(TestContext.CancellationTokenSource.Token);
        Assert.IsNotNull(tokenResponse);
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(tokenResponse.AccessToken);
        var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "role");
        var customerIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "customerId");
        Assert.IsNotNull(roleClaim);
        Assert.AreEqual("CUSTOMER_USER", roleClaim.Value);
        Assert.IsNotNull(customerIdClaim);
        Assert.AreEqual("47e311e1-d8d0-4746-b12c-f72b8aba57ca", customerIdClaim.Value);
    }

    #endregion

    #region Refresh

    [TestMethod]
    public async Task Refresh_WithValidToken_ReturnsOk()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            CpfNumber = "12345678909",
            Password = "5eCre+Key",
        };
        var loginResponse =
            await _client.PostAsJsonAsync("api/auth/login", loginRequest, TestContext.CancellationTokenSource.Token);
        var loginData = await loginResponse.Content.ReadFromJsonAsync<TokenResponse>(TestContext.CancellationTokenSource.Token);

        var refreshRequest = new RefreshTokenRequest
        {
            RefreshToken = loginData!.RefreshToken,
        };

        // Act
        var response = await _client.PostAsJsonAsync("api/auth/refresh", refreshRequest, TestContext.CancellationTokenSource.Token);

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>(TestContext.CancellationTokenSource.Token);
        Assert.IsNotNull(tokenResponse);
        Assert.IsFalse(string.IsNullOrWhiteSpace(tokenResponse.AccessToken));
        Assert.IsFalse(string.IsNullOrWhiteSpace(tokenResponse.RefreshToken));
    }

    [TestMethod]
    public async Task Refresh_WithInvalidToken_ReturnsUnauthorized()
    {
        // Arrange
        var request = new RefreshTokenRequest
        {
            RefreshToken = "invalid-refresh-token",
        };

        // Act
        var response = await _client.PostAsJsonAsync("api/auth/refresh", request, TestContext.CancellationTokenSource.Token);

        // Assert
        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion
}
